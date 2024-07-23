# openfeature-flagd-with-dotnet-demo

Demo project showcasing OpenFeature's Flagd integration with a .NET application for dynamic feature management and efficient deployment.

## DEMO 04

> refernce: https://openfeature.dev/docs/tutorials/open-feature-operator/quick-start
> 
### Start a New Minikube Cluster

1. **Start Minikube**:
```bash
minikube start --driver=docker
# minikube stop
# minikube delete
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
If open-feature-operator-controller-manager is pending after installation, run again
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
#remove kubectl -n default delete -f app.yaml
```

4. **Port Forward**
```bash
kubectl port-forward services/open-feature-demo-app-service 30000:5005
```
        