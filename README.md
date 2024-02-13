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
  * HaKafkaNet Transformer
  * HaKafkaNet State Manager
* Home Assistant Yellow


[This](https://github.com/leosperry/MyHome/tree/main/MyHome/Automations) folder has my working automations.

Join me on the new [Discord Server](https://discord.gg/RaGu72RbCt) for HaKafkaNet.

You may also note that I have set up the Transformer and StateManager to run in seperate projects. There are a few benefits to this approach.

Happy Automating!
