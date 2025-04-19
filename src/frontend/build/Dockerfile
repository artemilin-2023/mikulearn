FROM node:20-alpine AS build-base

FROM build-base AS build-deps

WORKDIR /app

RUN --mount=type=bind,source=package.json,target=package.json \
  --mount=type=bind,source=package-lock.json,target=package-lock.json \
  --mount=type=cache,target=/root/.npm \
  npm ci --omit=dev

FROM build-base AS build-builder

WORKDIR /app

COPY --from=build-deps /app/node_modules ./node_modules
COPY . .
RUN npm run build

FROM build-base AS dev

WORKDIR /app

RUN --mount=type=bind,source=package.json,target=package.json \
  --mount=type=bind,source=package-lock.json,target=package-lock.json \
  --mount=type=cache,target=/root/.npm \
  npm ci --include=dev
USER node
COPY . .
CMD ["npm", "run", "dev"]

FROM nginx:alpine AS deploy

COPY --from=build-builder --chown=nginx:nginx /app/dist /usr/share/nginx/html
COPY build/nginx.conf /etc/nginx/nginx.conf
