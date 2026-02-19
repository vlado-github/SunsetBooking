# SunsetBooking
Sample API for finding the closest hotels by proximity 

## Prerequisite

1. Download and install [docker compose](https://docs.docker.com/compose/install/)

2. Edit Hosts file in elevated mode and add following:
```bash
## Windows (C:\Windows\System32\drivers\etc)

127.0.0.1 keycloak.local
```

```bash
## Mac and Linux (/etc/hosts)
127.0.0.1 keycloak.local 
```

## Docker

From the repository's root directory, run command in terminal:
```bash
docker compose up -d
```

### Open API docs

- Docker container: [http://localhost:5013/swagger/index.html](http://localhost:5013/swagger/index.html)
