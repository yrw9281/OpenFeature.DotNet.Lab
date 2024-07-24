# Openfeature Turtorial with Flagd and .NET

Demo project showcasing OpenFeature's Flagd integration with a .NET application for dynamic feature management and efficient deployment.

## Preparation

1. **Mac or Linux recommended**

I preferred use the Mac or Linux to run this demo since Flagd has some issues with WSL/Hyper-V in windows system. Here is the description:

> use docker:
    _Note - In Windows, use WSL system for both the file location and Docker runtime. Mixed file systems don't work and this is a [limitation of Docker (https://github.com/docker/for-win/issues/8479)_

2. **Docker**

Make sure you installed docker in your device.

3. **VSCode recommended**

To avoid polluting your development environment, you can use a DevContainer configuration in Visual Studio Code (VSCode).

## DEMO 01 - Flagd Provide by File

This demo shows the simplest case for the integration of OpenFeature and Flagd.

1. **Start**:

```bash
docker compose up -d
#docker compose down
```

2. **Check**

You can change the `defaultVariant` to `off` in Flagd.json and check the result change on http://localhost:5001.

## DEMO 02 - Flagd Provide by Api
 
This demo shows the Flagd can use API as data source.

1. **Start**:

```bash
docker compose up -d
#docker compose down
```

2. **Check**

You can change the `defaultVariant` to `off` in Flagd.json and check the result change on http://localhost:5003.


## DEMO 03 - OpenFeature Evaluation Context

This demo shows how the Evaluation Context works in OpenFeature and Flagd. Additionally, providing the A/B performance test case with Prometheus and Grafana.

1. **Start**:

```bash
docker compose up -d
#docker compose down
```

2. **Run the test scripts**

```bash
./test.sh
```

3. **Check the result on Grafana**

Browse http://localhost:3000 and login in with `admin`/`grafana`, the grafana default user.
Go to Dashboards -> Experiment and Normal API Latency. You can see the A/B performance test graph.

## DEMO 04 - OpenFeature on Kubenetes

This demo shows how to create a minikube cluster in docker and setup the open-feature-operator to use.

> refernce: https://openfeature.dev/docs/tutorials/open-feature-operator/quick-start

### Start a New Minikube Cluster

1. **Start Minikube**:

```bash
minikube start --driver=docker
#minikube stop
#minikube delete
```

2. **Check Cluster Status**:

```bash
minikube status
```

3. **Verify the Configuration**:

```bash
kubectl config view
```

### Install OpenFeature Operator

1. **Install Cert-Manager**:

```bash
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.2/cert-manager.yaml && \
kubectl wait --timeout=60s --for condition=Available=True deploy --all -n 'cert-manager'
```

2. **Install OpenFeature Operator by helm**:

Install and Wait for a while till open-feature-operator-controller-manager is running.

```bash
helm repo add openfeature https://open-feature.github.io/open-feature-operator/ && \
helm repo update && \
helm upgrade --install open-feature-operator openfeature/open-feature-operator
```

### Deploy FeatureFlag and FeatureFlagSource

1. **Apply flag.yaml**

```bash
kubectl -n default apply -f flag.yaml
```

2. **Check**

```bash
kubectl get FeatureFlag
kubectl get FeatureFlagSource
```

### Deploy Application

1. **Load docker images to minikube**

```bash
docker build -t demo-04-app my-app/
minikube image load demo-04-app:latest
```

2. **Check**

```bash
minikube ssh -- docker images
```

3. **Apply app.yaml**

```bash
kubectl -n default apply -f app.yaml
#kubectl -n default delete -f app.yaml
```

4. **Check the Pod is Running**4

```bash
kubectl -n default get pods -l app=open-feature-demo
```

5. **Port Forward**

```bash
kubectl port-forward services/open-feature-demo-app-service 30000:5005
```

### Test the result

1. **Modify the flag.yaml**

Change the flags config as you want in flag.yaml.

```yaml
...
    flags:
      feature-a:
        variants:
          'on': true
          'off': false
        defaultVariant: 'on'
        state: ENABLED
      experiment:
        variants:
          'on': true
          'off': false
        defaultVariant: 'off'
        state: ENABLED
        targeting:
          if:
            - ends_with:
                - var: email
                - "@test.company.com"
            - 'on'
...
```

2. **Apply flag.yaml**

```bash
kubectl -n default apply -f flag.yaml
```

3. **Check**

You can check the result on browser or run test scripts.

```bash
./test.sh
```
