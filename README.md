# MikuLearn

\<DESCRIPTION: FILL ME>

## Install and run

The project can be run in `docker` using `docker compose`

1. `docker-compose.yaml` contains the definitions of all used services. An example production configuration is provided in `docker-compose-prod.yaml.example` as an override to highlight the required changes needed for the first build
  - `docker compose up -d --build` to run the project on `localhost`
  - `docker compose -f docker-compose.yaml -f docker-compose-prod.yaml up -d --build`

## Project architecture

A main [Caddy](https://caddyserver.com) acts as a tls-terminating reverse proxy for the frontend and the backend
The frontend is a static single-page app built using [React](https://react.dev) and [Vite](https://vite.dev). You can find more in [[src/frontend/README.md]]. Static files are server using [Nginx](https://nginx.org)
The backend is a C# [ASP.NET](https://learn.microsoft.com/ru-ru/aspnet/core) application which uses [PostgreSQL
](https://www.postgresql.org) as a main database, [Redis](https://redis.io) as a cache and session storage and [MinIO](https://min.io) to store user uploaded files.

## Configuration

1. Main infrastructure configuration is done on the ingress `caddy` instance using environment variables
  - `CADDY_EXTRA_DIRECTIVES` allows arbitrary statements to be inserted into the `Caddyfile`
    - specifically `import cloudflare_dns` allows the use of the `cloudflare-dns` ACME challenge. It is used for wildcard certificate enrolling and requires the `CLOUDFLARE_API_TOKEN` to be set to a `Zone.DNS Edit` scoped API token.
  - `CADDY_CERT_DOMAIN` specifies the domain name that will be present on the auto-issued certificate. Leave blank for a `localhost` certificate ([see more](https://caddyserver.com/docs/automatic-https#local-https))
  - `CADDY_HOST` specifies the expected application hostname. Leave blank to match any hostname, or when not using a wildcard certificate

2. Application itself is configured with `backend`'s environment variables. They directly modify the appsettings of the application ([see more](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-9.0#evcp))
  - `ALLOWED_CORS_HOSTS` directly affects the `Access-Control-Allow-Origin` response header ([see more](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/CORS#the_http_response_headers)). It is a list of allowed origins delimeted by `;`. Has to be set to the application hostname
  - `ConnectionStrings__OpenRouter` allows the application to use any OpenAI-compatible API like [OpenRouter](https://openrouter.ai/)

```
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⠤⢲⣦⡀⠀⣠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣀⣠⠖⠚⠓⠒⠒⠲⠿⣍⣛⣻⣦⣷⢠⣻⣀⠤⠤⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠔⡿⠓⠲⠬⢥⡤⠤⠤⠤⠤⣤⣀⣀⣈⣻⣿⣿⠓⠉⣀⡤⠔⢶⣻⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣤⢤⠞⠁⣼⠀⠀⠀⠀⠀⠀⠰⠶⢌⣽⡶⠟⢏⡁⢈⣉⣭⣷⡯⣝⠒⢦⣄⠙⠷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡾⣵⠿⣳⡟⠉⠉⠿⠀⠀⡀⠀⠀⣀⣀⣢⣾⣿⡚⠓⠒⠒⠒⠻⢿⣟⣧⡀⢠⣢⡻⠙⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡀⢿⣟⣲⢿⣷⢀⣤⣠⣄⣈⢿⣷⣤⣷⣻⡓⠒⠻⠭⢷⡄⠒⠲⢶⣦⣬⣻⣏⢦⡈⢿⣆⠘⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⢖⣩⠷⢎⣙⣿⣿⣼⡼⣿⣟⣽⣻⢯⢿⠞⣌⠙⢳⡕⠀⠙⠲⣶⠤⢄⡠⣄⡈⠛⣿⣿⣧⢳⣌⢻⡆⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⠗⠉⢀⣾⠟⠒⡿⣫⡿⡾⢫⢟⡏⠉⢸⡀⠑⢾⡆⢤⣻⣶⣤⣘⣲⣿⣶⡽⠿⣯⠲⣌⣇⢹⣷⣿⣆⠸⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⡼⠀⣿⢣⣧⠋⣸⠀⣰⢀⣿⣆⠀⠰⣄⠙⣧⣨⣿⣿⣿⣯⣿⣦⠘⣷⡌⢻⡄⠿⣆⠟⣆⢇⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⠀⠀⠀⠀⠀⠀⠀⠀⡇⡼⣹⠈⢣⠆⡇⣰⣻⣼⣿⣽⣷⣦⣹⣿⣟⣯⠉⠘⣿⠏⠃⠀⣷⣽⡿⣾⡹⣷⣹⣧⡈⢺⡀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⢾⠟⠉⠀⠀⠀⠀⠀⠀⠀⠀⣷⣱⣽⡀⢸⣆⢳⣿⣇⣷⡿⠖⠿⠷⢦⣙⣆⠩⡗⠀⠀⠀⠀⠀⡯⠿⠳⣟⠻⣽⣄⢧⠈⠳⣷⡀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⡇⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⣿⣿⣷⣸⣿⡀⣿⣅⠛⠇⠀⠀⠀⠀⢈⠙⠂⠀⠀⠀⠀⠀⣸⣿⠀⢠⣜⢦⠘⢿⣮⣷⡀⠀⠙⢦⡀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⢯⣇⠙⢄⣀⠀⠀⠀⢀⣀⡠⣴⣾⣫⠵⠛⣿⣻⠿⣷⣹⣾⣷⡠⡀⠀⠀⠀⠻⠦⠀⣀⠔⠀⠀⣴⣿⣸⣧⡀⠳⣝⠳⢴⡙⢞⣏⠢⡀⠀⠹⡄⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⢀⣀⣀⣈⣻⣤⣀⠈⠉⠻⠿⢿⣺⣿⡵⣶⢶⣦⠀⠘⣿⢷⣾⢿⣿⡋⢙⣮⣄⡀⠀⠀⠉⠈⠀⠀⢀⢞⣿⣼⣿⣿⣿⣆⡀⠙⢦⣝⣮⠛⢷⡙⢆⠀⣷⠀⠀⠀⠀
⠀⠀⠀⢠⡴⢟⣫⠤⠖⠒⠛⢛⣲⣿⣿⣿⠟⠋⢱⣿⣴⣷⣿⣹⢃⣀⣘⠃⢩⢿⣫⣥⣿⠧⢿⣿⣗⡶⣤⣄⣀⣴⣷⣻⣿⣿⠻⠘⣏⠛⢿⣦⣀⠱⢤⣉⡑⠛⠮⢿⣹⠀⠀⠀⠀
⠀⢠⢖⠵⠊⠁⢀⡤⠖⣲⣿⣿⣟⣻⣿⣋⠀⠀⠈⢿⣿⡿⠟⣵⣿⣿⣿⣷⡏⠈⢿⡿⣷⣤⣼⢿⣿⣿⢶⣯⣭⣵⣾⢿⣿⠿⢦⡀⢹⠀⠈⣏⠉⠉⢻⣶⣯⡑⠦⣄⠈⠳⣄⠀⠀
⠀⠹⠁⠀⠀⣞⢁⡾⢽⣯⣝⣛⣛⣯⣭⣽⣿⣷⣶⣤⣤⣴⣿⣿⣿⣿⣿⣿⡀⣀⣼⣀⠈⠙⠛⠷⣾⡿⢿⡟⣿⠟⠁⣈⣥⡴⠾⠷⢾⡄⠀⣿⠀⠀⠈⢇⠘⣿⣤⣀⠑⢦⡘⢧⠀
⠄⠀⠀⠀⠀⠈⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⢿⡿⠛⣻⢏⡽⣯⡄⠘⣏⣿⡏⠈⠙⠓⠶⣤⣀⠀⠙⢿⣿⡇⢀⡿⠋⠁⠀⢀⣀⣸⡇⠀⣟⣀⠀⠀⠀⠙⠃⣿⡎⠑⣤⡙⣌⡇
⠀⠀⠀⠀⠀⠀⠀⠙⢿⣿⣿⣿⣿⣿⣿⣿⡥⣖⡯⠖⢋⣡⡞⣼⣿⠀⠀⢹⣿⣷⠀⠀⣀⡀⠀⠉⠻⣦⠀⢿⣿⡿⠁⣠⡶⠟⠋⠉⠹⣆⣸⣿⣿⠀⣀⡤⣤⡀⣿⠇⠀⢸⠳⡜⡇
⠀⠀⠀⠀⠀⠀⠀⠀⣈⣿⣿⣿⣿⣿⣿⣏⢿⣗⠒⠊⠉⢸⠁⡿⡏⠀⢀⣿⡿⣁⠤⢶⣿⣽⡆⠀⠀⠘⢷⡈⠉⣡⠾⠋⠀⠀⣠⣆⢠⣿⣿⣿⡟⢀⣷⠒⢺⣧⡏⠀⠀⢸⠀⢹⣹
⠀⠀⠀⠀⠀⠀⡠⢪⠟⡽⠙⢶⣾⣿⣿⣿⣷⡻⣦⡀⠀⠀⢣⣇⡇⢀⡞⣸⠏⠁⠀⠀⡇⢻⡀⠀⠀⣾⣶⡟⣿⣥⡄⠀⠀⢠⣇⣼⡶⣿⣿⠋⣸⣾⣷⣚⣽⡟⠀⠀⠀⣏⠀⡼⣿
⠀⠀⠀⠀⢀⠎⡴⣣⣾⠟⣡⠞⢹⡿⣿⢿⣿⣿⣿⣷⣄⠀⠈⠻⡹⣼⠀⣿⡄⠀⠀⠀⠉⠻⣷⡀⠀⠙⠿⠇⣿⠿⠇⠀⠀⢠⡿⠋⢇⢹⣟⢷⠫⣿⣟⡿⠋⠀⠀⣠⣾⢞⡜⠁⡿
⠀⠀⠀⠀⡼⡼⣵⠏⡏⣰⠃⠀⢾⡇⠈⠻⣿⣮⡉⠹⣿⣧⣄⡀⠙⣇⠀⠸⣿⣄⡀⠀⠀⠀⠈⢉⣧⡴⠀⠠⡄⣀⡤⠤⠴⠋⠁⠀⠈⢻⣿⣾⣤⡿⠋⠀⠀⠀⣉⣽⠿⠋⠀⣰⠃
⠀⠀⠀⠀⣿⣽⠋⠀⡇⡇⠀⠀⠘⣷⢠⣶⢮⣻⣿⣦⠈⠛⠙⠹⣷⠘⢦⣤⣿⣳⣭⣑⣒⣒⣺⣿⢿⣀⣀⣀⣿⣧⣀⣀⣀⡤⠴⠒⣶⣿⣏⣾⡿⠁⠀⠀⠀⠉⠉⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⢻⡏⠀⠀⢳⣇⠀⠀⠀⠈⠈⣿⣾⣿⣿⣮⣿⣦⠀⠀⢿⣶⡫⣿⣿⣿⣿⣿⡹⣯⣊⠁⠉⠉⠉⠉⢙⣮⣷⣶⡤⣤⣶⣿⣿⣟⣾⢿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠸⣿⣦⡀⠀⠙⠦⠀⠀⢀⣼⠿⣽⣿⣯⣷⣼⡷⢾⣻⣾⣿⠞⢿⢻⡷⠻⣿⣏⡙⢝⣻⡾⡖⠒⣻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⣎⠣⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠈⠁⠀⠀⠀⠀⣴⣋⣉⣩⣿⣿⢿⣟⣷⣾⣯⡟⢱⡇⠀⠘⣿⠣⡀⣈⣻⣿⣿⣿⣷⣷⣶⣿⣿⣟⠋⡿⣹⣿⣿⣿⣿⢟⣏⠺⠿⠶⠭⠷⠂⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣿⣿⣿⣿⣿⣿⣿⢹⣮⣾⣿⡿⢋⣶⢸⡇⠀⠀⠈⢷⡑⠈⢻⣟⠛⠛⠿⠋⠙⠓⣭⣿⣹⣵⣿⣿⣿⣿⣿⠈⠻⣷⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠙⢟⣛⡭⠶⢾⡿⠃⠀⠀⢸⢹⣾⣇⠀⠀⠀⠀⠻⣟⣿⡏⠀⠀⠀⠀⢰⣿⣿⢿⡇⣿⣿⣿⣿⣟⡏⠀⠀⠀⠉⠙⠓⠒⠂⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡟⠀⠀⠀⢰⣿⢸⣿⢿⡀⠀⠀⠀⠀⢸⣿⣣⠀⠀⠀⢠⣿⡟⣷⣾⣿⢿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣿⠁⠀⠀⠀⠘⡿⡿⢿⣏⣧⠀⠀⠀⢀⣾⣿⠿⠀⢀⣴⣿⣿⢿⡹⡇⣿⠈⣿⣿⡟⣾⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⡇⠀⠀⠀⠀⢳⢧⠈⢿⣿⠀⠀⢀⣼⣿⠕⠁⣠⣾⡿⣻⠏⠀⢹⣹⣿⣆⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⣿⣷⠀⠀⠀⠀⠈⣏⢧⠀⢻⣧⠶⣋⠿⠋⣀⣾⣟⣿⠞⠁⠀⠀⠀⢯⣿⣿⣿⣿⣿⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⣿⣿⣇⠀⠀⠀⠀⠸⡜⣆⠈⢷⣿⠀⢠⣾⣿⠟⢻⡏⠀⠀⠀⠀⠀⠘⣿⣿⠏⣿⣿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡻⣿⣿⣦⡀⠀⠀⠀⢧⠘⣆⠈⢷⠒⠛⢻⡿⠄⠘⣧⠀⠀⠀⠀⠀⢰⣷⣹⠀⠙⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣷⣝⣿⣿⣷⣄⠀⠀⠈⢇⠸⡄⠈⣇⠀⠀⢻⣆⠀⠘⣇⠀⠀⣠⣾⣽⢹⡟⠀⢰⣷⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⣾⢻⠙⢿⣿⡇⠀⠀⠈⢧⢳⠀⢸⡀⠀⠀⢻⣆⠀⢻⡄⢠⣿⣯⣽⣓⣧⣤⠾⢹⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣼⡇⠀⠙⣿⠀⠀⠀⠈⢏⢧⠀⣇⠀⠀⠀⢻⣆⠀⢷⠘⣿⣮⡻⣿⠁⠀⢀⣯⣿⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⣧⡀⠀⠀⠀⠀⠀⠀⠘⡾⡄⢸⠀⠀⠀⠈⢻⣆⠈⢷⣷⣾⣟⣻⣶⣿⡿⠛⢿⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣿⣿⣿⣦⣀⠀⠀⠀⠀⠀⢳⣇⠘⡆⠀⠀⠀⠀⠻⣶⢺⡏⢹⡟⠀⠉⠁⠀⠀⠀⢩⣍⠙⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⣟⣿⣿⣿⣧⠀⢀⣤⣤⡸⣸⠀⡇⠀⠀⠀⠀⠀⠙⣿⣷⢡⣿⣶⣄⠀⠀⣄⠀⣦⣍⠙⢳⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⡿⢿⡙⠿⠀⢸⡟⢲⣷⡿⡄⢿⠀⠀⠀⠀⠀⠀⠘⣏⢸⣇⣍⢻⣷⣀⢻⣦⣤⣌⡙⣦⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣯⢷⠀⠀⠸⡇⠀⢿⣧⡇⢸⠀⠀⠀⠀⠀⠀⠀⣼⢸⣿⣯⠀⣿⣿⣷⣝⣿⣻⣿⣼⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⣎⣧⠀⠐⣇⠀⠘⣽⣿⡸⡆⠀⠀⠀⠀⠀⠀⠈⠋⠈⠻⣼⣿⢶⢿⣿⣯⡻⣿⣿⣾⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⡼⡆⠀⣿⡀⠀⣿⢿⡇⢷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⣾⢧⠈⠙⠿⣮⠟⠉⠙⢦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢿⢻⢠⣿⡇⠀⠹⣄⢳⣜⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⡏⠪⠳⢤⣄⣀⠀⠀⠀⠈⢣⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣎⣿⡟⡇⠀⠀⠈⢻⣿⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢳⡄⠀⠀⠀⠙⢧⡀⢀⡄⠈⡇⠀⠀⠀⠀⠀⠀⠀⠀
```
