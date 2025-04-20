FROM node:20-alpine AS build-base

FROM build-base AS build-deps

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci


FROM build-base AS build-builder

WORKDIR /app

COPY --from=build-deps /app/node_modules ./node_modules
COPY . .
RUN npm run build


FROM build-deps AS dev

COPY . .

CMD ["npm", "run", "dev", "--", "--host", "--port", "3000"]


FROM nginx:alpine AS deploy

COPY --from=build-builder --chown=nginx:nginx /app/dist /usr/share/nginx/html
COPY build/nginx.conf /etc/nginx/nginx.conf
