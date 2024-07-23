#!/bin/bash

i=0

while true; do
    i=$((i + 1))

    for email in "me@test.company.com" "me@company.com"; do
        RESPONSE=$(curl -s -H "email: $email" -X GET http://localhost:5004/world)
        echo "Request $i: Called API from ${email%@*}"
        echo "Response Body: $RESPONSE"
        echo "---------------------------------------"
    done

    sleep 0.2
done