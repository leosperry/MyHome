# MyHome
This is the repository for automations I use in my home. I have decided to make it public so that users of [HaKafkaNet](https://github.com/leosperry/ha-kafka-net) can see more complex working examples.

## Environment
These are some of the things I use in my set up
* Micro-PC running Linux with mobile processor, 32GB RAM, 2TB SSd, and Google Coral USB (for Frigate NVR) witch draws about 10W total.
  * This little beast handles all my docker containers and is never taxed for resources
* Portainer - very handy for managing docker containers. Containers I have running regularly:
  * Frigate NVR
  * Kafka
  * Kafka UI
  * Redis Cache
  * HaKafkaNet
  * OTEL Collector
  * Grafana Loki
  * Grafana Tempo
  * Grafana Prometheus
  * Traefik
* Home Assistant Yellow

Some key features
* Traefik is a reverse proxy that allow all my services to run http, and be exposed with domain names as https. This allows me to easily serve the HaKafkaNet dashboard up via my Home Assistent dashboard
* HaKafkaNet [open telemetry intstrumentation](https://github.com/leosperry/ha-kafka-net/wiki/Open-Telemetry-Instrumentation) sends data to the OTEL collector, which in turn sends datat to the Grafana stack. Grafana itself is served up by Home Assistent on the Yellow.

Join me on the new [Discord Server](https://discord.gg/RaGu72RbCt) for HaKafkaNet.


Happy Automating!
