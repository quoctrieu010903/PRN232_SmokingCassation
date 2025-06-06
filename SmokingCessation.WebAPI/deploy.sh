#!/bin/bash

# Cấu hình
IMAGE_NAME="smokingcessationwebapi"
DOCKERHUB_USERNAME="quoctrieu60"    # 👉 đổi tên tại đây
TAG="latest"
PORT=5081

# Đăng nhập Docker Hub
echo "==> Docker login"
docker login

# Build image
echo "==> Building Docker image..."
docker build -t $IMAGE_NAME .

# Tag image theo Docker Hub
echo "==> Tagging Docker image..."
docker tag $IMAGE_NAME $DOCKERHUB_USERNAME/$IMAGE_NAME:$TAG

# Push image lên Docker Hub
echo "==> Pushing image to Docker Hub..."
docker push $DOCKERHUB_USERNAME/$IMAGE_NAME:$TAG

# Xóa container cũ nếu có
echo "==> Stopping and removing old container (if exists)..."
docker stop $IMAGE_NAME 2>/dev/null
docker rm $IMAGE_NAME 2>/dev/null

# Run container với port 5081
echo "==> Running new container on port $PORT..."
docker run -d -p $PORT:$PORT --name $IMAGE_NAME $DOCKERHUB_USERNAME/$IMAGE_NAME:$TAG

echo "✅ Deployment complete. App is running on http://localhost:$PORT"
