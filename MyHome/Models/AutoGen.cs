/* cSpell:disable */

public class Labels
{ 
    public const string Bedtimeoff = "bedtimeoff";
    public const string OfficeSwitches = "office_switches";
    public const string BasementLights = "basement_lights";
}

public class Areas
{  
    public const string LivingRoom = "living_room"; 
    public const string Kitchen = "kitchen"; 
    public const string Bedroom = "bedroom"; 
    public const string Lounge = "lounge"; 
    public const string FrontRoom = "front_room"; 
    public const string Entry = "entry"; 
    public const string LyraBedroom = "lyra_bedroom"; 
    public const string Basement = "basement"; 
    public const string AsherBedroom = "asher_bedroom"; 
    public const string LoganBedroom = "logan_bedroom"; 
    public const string Garage = "garage"; 
    public const string Office = "office"; 
    public const string Virtual = "virtual"; 
    public const string Unknown = "unknown"; 
    public const string General = "general"; 
    public const string BackYard = "back_yard"; 
    public const string Laundry = "laundry"; 
    public const string FamilyRoom = "family_room"; 
    public const string AsherRoom = "asher_room"; 
    public const string UnnamedRoom = "unnamed_room";
}

public class Automation
{  
    public const string BackupFailure = "automation.backup_failure"; 
    public const string LyraMedicine = "automation.lyra_medicine"; 
    public const string RunWatchman = "automation.run_watchman"; 
    public const string CpuHigh = "automation.cpu_high"; 
    public const string HakafkanetSendNotificationUpdates = "automation.hakafkanet_send_notification_updates"; 
    public const string HakafkanetNotifyOnStartupAndShutdown = "automation.hakafkanet_notify_on_startup_and_shutdown"; 
    public const string HakafkanetNotifyOnStartupAndShutdown2 = "automation.hakafkanet_notify_on_startup_and_shutdown_2";
}

public class Binary_Sensor
{  
    public const string RemoteUi = "binary_sensor.remote_ui"; 
    public const string MbrMotionGroup = "binary_sensor.mbr_motion_group"; 
    public const string Rachelphonebattlowhelper = "binary_sensor.rachelphonebattlowhelper"; 
    public const string KazulLightTimeSensor = "binary_sensor.kazul_light_time_sensor"; 
    public const string LyraBrushHair = "binary_sensor.lyra_brush_hair"; 
    public const string HouseActiveTimesOfDay = "binary_sensor.house_active_times_of_day"; 
    public const string OfficeIlluminanceTrend = "binary_sensor.office_illuminance_trend"; 
    public const string RokuUltraHeadphonesConnected = "binary_sensor.roku_ultra_headphones_connected"; 
    public const string RokuUltraSupportsAirplay = "binary_sensor.roku_ultra_supports_airplay"; 
    public const string RokuUltraSupportsEthernet = "binary_sensor.roku_ultra_supports_ethernet"; 
    public const string RokuUltraSupportsFindRemote = "binary_sensor.roku_ultra_supports_find_remote"; 
    public const string EsphomeLivingRoomStatus = "binary_sensor.esphome_living_room_status"; 
    public const string EsphomeLivingRoomPresence = "binary_sensor.esphome_living_room_presence"; 
    public const string EsphomeLivingRoomMovingTarget = "binary_sensor.esphome_living_room_moving_target"; 
    public const string EsphomeLivingRoomStillTarget = "binary_sensor.esphome_living_room_still_target"; 
    public const string EsphomekitchenmotionStatus = "binary_sensor.esphomekitchenmotion_status"; 
    public const string EsphomekitchenmotionPresence = "binary_sensor.esphomekitchenmotion_presence"; 
    public const string EsphomekitchenmotionMovingTarget = "binary_sensor.esphomekitchenmotion_moving_target"; 
    public const string LivingRoomButtonsLowBatteryLevel = "binary_sensor.living_room_buttons_low_battery_level"; 
    public const string LoungeButtonsLowBatteryLevel = "binary_sensor.lounge_buttons_low_battery_level"; 
    public const string KazulPowerStripOverLoadDetected = "binary_sensor.kazul_power_strip_over_load_detected"; 
    public const string BasementMotionTamperingProductCoverRemoved = "binary_sensor.basement_motion_tampering_product_cover_removed"; 
    public const string BasementMotionMotionDetection = "binary_sensor.basement_motion_motion_detection"; 
    public const string BasementMotionLowBatteryLevel = "binary_sensor.basement_motion_low_battery_level"; 
    public const string KazulTempHumidityOverheatDetected = "binary_sensor.kazul_temp_humidity_overheat_detected"; 
    public const string KazulTempHumidityUnderheatDetected = "binary_sensor.kazul_temp_humidity_underheat_detected"; 
    public const string KazulTempHumidityMoistureAlarm = "binary_sensor.kazul_temp_humidity_moisture_alarm"; 
    public const string KazulTempHumidityLowBatteryLevel = "binary_sensor.kazul_temp_humidity_low_battery_level"; 
    public const string Garage1TiltSensorStateAny = "binary_sensor.garage_1_tilt_sensor_state_any"; 
    public const string Garage1TiltTamperingProductCoverRemoved = "binary_sensor.garage_1_tilt_tampering_product_cover_removed"; 
    public const string Garage1TiltLowBatteryLevel = "binary_sensor.garage_1_tilt_low_battery_level"; 
    public const string Garage2TiltSensorStateAny = "binary_sensor.garage_2_tilt_sensor_state_any"; 
    public const string Garage2TiltTamperingProductCoverRemoved = "binary_sensor.garage_2_tilt_tampering_product_cover_removed"; 
    public const string Garage2TiltLowBatteryLevel = "binary_sensor.garage_2_tilt_low_battery_level"; 
    public const string _4In1SensorTamperingProductCoverRemoved = "binary_sensor.4_in_1_sensor_tampering_product_cover_removed"; 
    public const string _4In1SensorMotionDetection = "binary_sensor.4_in_1_sensor_motion_detection"; 
    public const string _4In1SensorLowBatteryLevel = "binary_sensor.4_in_1_sensor_low_battery_level"; 
    public const string PlantPlug1OverCurrentDetected = "binary_sensor.plant_plug_1_over_current_detected"; 
    public const string PlantPlug1OverVoltageDetected = "binary_sensor.plant_plug_1_over_voltage_detected"; 
    public const string PlantPlug1OverLoadDetected = "binary_sensor.plant_plug_1_over_load_detected"; 
    public const string Dv102683GLaundryEndOfCycle = "binary_sensor.dv102683g_laundry_end_of_cycle"; 
    public const string Dv102683GLaundryDoor = "binary_sensor.dv102683g_laundry_door"; 
    public const string Dv102683GLaundryRemoteStatus = "binary_sensor.dv102683g_laundry_remote_status"; 
    public const string Dv102683GLaundryDryerWasherlinkStatus = "binary_sensor.dv102683g_laundry_dryer_washerlink_status"; 
    public const string Dv102683GLaundryDryerLevelSensorDisabled = "binary_sensor.dv102683g_laundry_dryer_level_sensor_disabled"; 
    public const string Av339078NLaundryEndOfCycle = "binary_sensor.av339078n_laundry_end_of_cycle"; 
    public const string Av339078NLaundryDoor = "binary_sensor.av339078n_laundry_door"; 
    public const string Av339078NLaundryRemoteStatus = "binary_sensor.av339078n_laundry_remote_status"; 
    public const string Av339078NLaundryWasherDoorLock = "binary_sensor.av339078n_laundry_washer_door_lock"; 
    public const string Av339078NLaundryWasherTimesaver = "binary_sensor.av339078n_laundry_washer_timesaver"; 
    public const string Av339078NLaundryWasherPowersteam = "binary_sensor.av339078n_laundry_washer_powersteam"; 
    public const string Av339078NLaundryWasherPrewash = "binary_sensor.av339078n_laundry_washer_prewash"; 
    public const string Rt149699DoorStatusAnyOpen = "binary_sensor.rt149699_door_status_any_open"; 
    public const string Rt149699IceMakerControlStatusFridge = "binary_sensor.rt149699_ice_maker_control_status_fridge"; 
    public const string DoorbellRepeater579AMotionSensor = "binary_sensor.doorbell_repeater_579a_motion_sensor"; 
    public const string BasementStairMotionAq2Occupancy2 = "binary_sensor.basement_stair_motion_aq2_occupancy_2"; 
    public const string BasementStairMotionAq2Motion2 = "binary_sensor.basement_stair_motion_aq2_motion_2"; 
    public const string BackDoorContactOpening = "binary_sensor.back_door_contact_opening"; 
    public const string InsideGarageDoorContactOpening = "binary_sensor.inside_garage_door_contact_opening"; 
    public const string LumiLumiSensorMotionAq2Occupancy3 = "binary_sensor.lumi_lumi_sensor_motion_aq2_occupancy_3"; 
    public const string LumiLumiSensorMotionAq2Motion3 = "binary_sensor.lumi_lumi_sensor_motion_aq2_motion_3"; 
    public const string FrontDoorContactOpening = "binary_sensor.front_door_contact_opening"; 
    public const string OfficeDoorOpening = "binary_sensor.office_door_opening"; 
    public const string Garage2ContactOpening = "binary_sensor.garage_2_contact_opening"; 
    public const string Garage1ContactOpening = "binary_sensor.garage_1_contact_opening"; 
    public const string OfficeMotionOccupancy = "binary_sensor.office_motion_occupancy"; 
    public const string OfficeMotionMotion = "binary_sensor.office_motion_motion"; 
    public const string BackHallCoatClosetContactOpening = "binary_sensor.back_hall_coat_closet_contact_opening"; 
    public const string MbrMotion1Motion = "binary_sensor.mbr_motion_1_motion"; 
    public const string MbrMotion2Motion = "binary_sensor.mbr_motion_2_motion"; 
    public const string BasementMotion2Motion = "binary_sensor.basement_motion_2_motion"; 
    public const string BackupsStale = "binary_sensor.backups_stale";
}

public class Button
{  
    public const string AqaraHubM2Identify = "button.aqara_hub_m2_identify"; 
    public const string AqaraSmartLockU100Identify = "button.aqara_smart_lock_u100_identify"; 
    public const string MbrDadSideSwitchIdentify = "button.mbr_dad_side_switch_identify"; 
    public const string FrontRoomComputerLampIdentify = "button.front_room_computer_lamp_identify"; 
    public const string UswFlexMiniRestart = "button.usw_flex_mini_restart"; 
    public const string PicardRestart = "button.picard_restart"; 
    public const string HeimdallRestart = "button.heimdall_restart"; 
    public const string PicardPort1PowerCycle = "button.picard_port_1_power_cycle"; 
    public const string PicardPort2PowerCycle = "button.picard_port_2_power_cycle"; 
    public const string PicardPort3PowerCycle = "button.picard_port_3_power_cycle"; 
    public const string PicardPort4PowerCycle = "button.picard_port_4_power_cycle"; 
    public const string PicardPort5PowerCycle = "button.picard_port_5_power_cycle"; 
    public const string PicardPort6PowerCycle = "button.picard_port_6_power_cycle"; 
    public const string PicardPort7PowerCycle = "button.picard_port_7_power_cycle"; 
    public const string PicardPort8PowerCycle = "button.picard_port_8_power_cycle"; 
    public const string HeimdallPort1PowerCycle = "button.heimdall_port_1_power_cycle"; 
    public const string HeimdallPort2PowerCycle = "button.heimdall_port_2_power_cycle"; 
    public const string HeimdallPort3PowerCycle = "button.heimdall_port_3_power_cycle"; 
    public const string HeimdallPort4PowerCycle = "button.heimdall_port_4_power_cycle"; 
    public const string HeimdallPort5PowerCycle = "button.heimdall_port_5_power_cycle"; 
    public const string HeimdallPort6PowerCycle = "button.heimdall_port_6_power_cycle"; 
    public const string HeimdallPort7PowerCycle = "button.heimdall_port_7_power_cycle"; 
    public const string HeimdallPort8PowerCycle = "button.heimdall_port_8_power_cycle"; 
    public const string EsphomeLivingRoomLd2450FactoryReset = "button.esphome_living_room_ld2450_factory_reset"; 
    public const string EsphomeLivingRoomLd2450Restart = "button.esphome_living_room_ld2450_restart"; 
    public const string EsphomekitchenmotionLd2450FactoryReset = "button.esphomekitchenmotion_ld2450_factory_reset"; 
    public const string EsphomekitchenmotionLd2450Restart = "button.esphomekitchenmotion_ld2450_restart"; 
    public const string Node2Ping = "button.node_2_ping"; 
    public const string Node3Ping = "button.node_3_ping"; 
    public const string Node4Ping = "button.node_4_ping"; 
    public const string Node5Ping = "button.node_5_ping"; 
    public const string Node6Ping = "button.node_6_ping"; 
    public const string Node7Ping = "button.node_7_ping"; 
    public const string Node8Ping = "button.node_8_ping"; 
    public const string Node10Ping = "button.node_10_ping"; 
    public const string Node12Ping = "button.node_12_ping"; 
    public const string Node14Ping = "button.node_14_ping"; 
    public const string Node15Ping = "button.node_15_ping"; 
    public const string Node16Ping = "button.node_16_ping"; 
    public const string Node17Ping = "button.node_17_ping"; 
    public const string Node18Ping = "button.node_18_ping"; 
    public const string Node20Ping = "button.node_20_ping"; 
    public const string Node21Ping = "button.node_21_ping"; 
    public const string Node22Ping = "button.node_22_ping"; 
    public const string Node23Ping = "button.node_23_ping"; 
    public const string Node24Ping = "button.node_24_ping"; 
    public const string Node30Ping = "button.node_30_ping"; 
    public const string Node31Ping = "button.node_31_ping"; 
    public const string Node32Ping = "button.node_32_ping"; 
    public const string Node33Ping = "button.node_33_ping"; 
    public const string Node34Ping = "button.node_34_ping"; 
    public const string Node35Ping = "button.node_35_ping"; 
    public const string Node38Ping = "button.node_38_ping"; 
    public const string Node39Ping = "button.node_39_ping"; 
    public const string OutsideDualPlugPing = "button.outside_dual_plug_ping"; 
    public const string BackHallLightIdentify = "button.back_hall_light_identify"; 
    public const string DiningRoomLightsIdentify = "button.dining_room_lights_identify"; 
    public const string PeacockLampIdentify = "button.peacock_lamp_identify"; 
    public const string BackPorchLightIdentify = "button.back_porch_light_identify"; 
    public const string BackFloodIdentify = "button.back_flood_identify"; 
    public const string KazulPowerStripResetAccumulatedValues = "button.kazul_power_strip_reset_accumulated_values"; 
    public const string KazulPowerStripIdlePowerManagementOverLoadStatus = "button.kazul_power_strip_idle_power_management_over_load_status"; 
    public const string KazulPowerStripResetAccumulatedValues1 = "button.kazul_power_strip_reset_accumulated_values_1"; 
    public const string KazulPowerStripResetAccumulatedValues2 = "button.kazul_power_strip_reset_accumulated_values_2"; 
    public const string KazulPowerStripResetAccumulatedValues3 = "button.kazul_power_strip_reset_accumulated_values_3"; 
    public const string KazulPowerStripResetAccumulatedValues4 = "button.kazul_power_strip_reset_accumulated_values_4"; 
    public const string KazulPowerStripResetAccumulatedValues5 = "button.kazul_power_strip_reset_accumulated_values_5"; 
    public const string BasementStairLightIdentify = "button.basement_stair_light_identify"; 
    public const string BasementMotionIdleHomeSecurityCoverStatus = "button.basement_motion_idle_home_security_cover_status"; 
    public const string BasementMotionIdleHomeSecurityMotionSensorStatus = "button.basement_motion_idle_home_security_motion_sensor_status"; 
    public const string KazulTempHumidityIdleHeatAlarmHeatSensorStatus = "button.kazul_temp_humidity_idle_heat_alarm_heat_sensor_status"; 
    public const string KazulTempHumidityIdleWeatherAlarmMoistureAlarmStatus = "button.kazul_temp_humidity_idle_weather_alarm_moisture_alarm_status"; 
    public const string KazulTempHumidityIdentify = "button.kazul_temp_humidity_identify"; 
    public const string Garage1TiltIdleHomeSecurityCoverStatus = "button.garage_1_tilt_idle_home_security_cover_status"; 
    public const string Garage2TiltIdleHomeSecurityCoverStatus = "button.garage_2_tilt_idle_home_security_cover_status"; 
    public const string _4In1SensorIdleHomeSecurityCoverStatus = "button.4_in_1_sensor_idle_home_security_cover_status"; 
    public const string _4In1SensorIdleHomeSecurityMotionSensorStatus = "button.4_in_1_sensor_idle_home_security_motion_sensor_status"; 
    public const string _4In1SensorIdentify = "button.4_in_1_sensor_identify"; 
    public const string PlantPlug1ResetAccumulatedValues = "button.plant_plug_1_reset_accumulated_values"; 
    public const string PlantPlug1IdlePowerManagementOverCurrentStatus = "button.plant_plug_1_idle_power_management_over_current_status"; 
    public const string PlantPlug1IdlePowerManagementOverVoltageStatus = "button.plant_plug_1_idle_power_management_over_voltage_status"; 
    public const string PlantPlug1IdlePowerManagementOverLoadStatus = "button.plant_plug_1_idle_power_management_over_load_status"; 
    public const string PlantPlug1Identify = "button.plant_plug_1_identify"; 
    public const string MbrDresserPlugIdentify = "button.mbr_dresser_plug_identify"; 
    public const string OutsideDualPlugIdentify = "button.outside_dual_plug_identify"; 
    public const string DoorbellRepeater579AIdentify = "button.doorbell_repeater_579a_identify"; 
    public const string OfficeFanIdentify = "button.office_fan_identify"; 
    public const string BasementStairMotionAq2Identify2 = "button.basement_stair_motion_aq2_identify_2"; 
    public const string BackDoorContactIdentify = "button.back_door_contact_identify"; 
    public const string InsideGarageDoorContactIdentify = "button.inside_garage_door_contact_identify"; 
    public const string LumiLumiSensorMotionAq2Identify3 = "button.lumi_lumi_sensor_motion_aq2_identify_3"; 
    public const string FrontDoorContactIdentify = "button.front_door_contact_identify"; 
    public const string OfficeDoorIdentify = "button.office_door_identify"; 
    public const string OfficeLedIdentify = "button.office_led_identify"; 
    public const string Garage2ContactIdentify = "button.garage_2_contact_identify"; 
    public const string Garage1ContactIdentify = "button.garage_1_contact_identify"; 
    public const string MonkeyIdentify = "button.monkey_identify"; 
    public const string OfficeMotionIdentify = "button.office_motion_identify"; 
    public const string BackHallCoatClosetContactIdentify = "button.back_hall_coat_closet_contact_identify";
}

public class Calendar
{  
    public const string HouseTasks = "calendar.house_tasks"; 
    public const string PhasesOfTheMoon = "calendar.phases_of_the_moon"; 
    public const string LeosperryGmailCom = "calendar.leosperry_gmail_com"; 
    public const string Sperryfamily = "calendar.sperryfamily";
}

public class Camera
{  
    public const string DoorbellRepeater579A = "camera.doorbell_repeater_579a";
}

public class Conversation
{  
    public const string HomeAssistant = "conversation.home_assistant";
}

public class Counter
{  
    public const string DemoCounter = "counter.demo_counter";
}

public class Device_Tracker
{  
    public const string EsphomeLivingRoom = "device_tracker.esphome_living_room"; 
    public const string Wiz79A59C = "device_tracker.wiz_79a59c"; 
    public const string UnifiDefault542A1B5F0B2C = "device_tracker.unifi_default_54_2a_1b_5f_0b_2c"; 
    public const string Wiz79Aab4 = "device_tracker.wiz_79aab4"; 
    public const string Wiz8D7D54 = "device_tracker.wiz_8d7d54"; 
    public const string UnifiDefault38420B98D42A = "device_tracker.unifi_default_38_42_0b_98_d4_2a"; 
    public const string UnifiDefault38420B98D37A = "device_tracker.unifi_default_38_42_0b_98_d3_7a"; 
    public const string Wiz79897C = "device_tracker.wiz_79897c"; 
    public const string Rokuultra = "device_tracker.rokuultra"; 
    public const string UnifiDefault68B6B333CcF0 = "device_tracker.unifi_default_68_b6_b3_33_cc_f0"; 
    public const string Wiz79A9C0 = "device_tracker.wiz_79a9c0"; 
    public const string Samsung = "device_tracker.samsung"; 
    public const string UswFlexMini = "device_tracker.usw_flex_mini"; 
    public const string Leonard = "device_tracker.leonard"; 
    public const string SmF721U = "device_tracker.sm_f721u"; 
    public const string Leophone = "device_tracker.leophone"; 
    public const string _4S4Wmaed9L3477167 = "device_tracker.4s4wmaed9l3477167"; 
    public const string Sperrylaser = "device_tracker.sperrylaser"; 
    public const string UswMini = "device_tracker.usw_mini";
}

public class Event
{  
    public const string OfficeDisplayLightsScene001 = "event.office_display_lights_scene_001"; 
    public const string OfficeDisplayLightsScene002 = "event.office_display_lights_scene_002"; 
    public const string BackHallLightScene001 = "event.back_hall_light_scene_001"; 
    public const string BackHallLightScene002 = "event.back_hall_light_scene_002"; 
    public const string KitchenLightsEventValue = "event.kitchen_lights_event_value"; 
    public const string KitchenLightsScene001 = "event.kitchen_lights_scene_001"; 
    public const string KitchenLightsScene002 = "event.kitchen_lights_scene_002"; 
    public const string DiningRoomLightsScene001 = "event.dining_room_lights_scene_001"; 
    public const string DiningRoomLightsScene002 = "event.dining_room_lights_scene_002"; 
    public const string DiningRoomLightsScene003 = "event.dining_room_lights_scene_003"; 
    public const string DiningRoomLightsScene004 = "event.dining_room_lights_scene_004"; 
    public const string DiningRoomLightsScene005 = "event.dining_room_lights_scene_005"; 
    public const string DiningRoomLightsScene006 = "event.dining_room_lights_scene_006"; 
    public const string LivingRoomButtonsScene001 = "event.living_room_buttons_scene_001"; 
    public const string LivingRoomButtonsScene002 = "event.living_room_buttons_scene_002"; 
    public const string LivingRoomButtonsScene003 = "event.living_room_buttons_scene_003"; 
    public const string LivingRoomButtonsScene004 = "event.living_room_buttons_scene_004"; 
    public const string LivingRoomButtonsScene005 = "event.living_room_buttons_scene_005"; 
    public const string LivingRoomButtonsScene006 = "event.living_room_buttons_scene_006"; 
    public const string LivingRoomButtonsScene007 = "event.living_room_buttons_scene_007"; 
    public const string LivingRoomButtonsScene008 = "event.living_room_buttons_scene_008"; 
    public const string LoungeButtonsScene001 = "event.lounge_buttons_scene_001"; 
    public const string LoungeButtonsScene002 = "event.lounge_buttons_scene_002"; 
    public const string LoungeButtonsScene003 = "event.lounge_buttons_scene_003"; 
    public const string LoungeButtonsScene004 = "event.lounge_buttons_scene_004"; 
    public const string LoungeButtonsScene005 = "event.lounge_buttons_scene_005"; 
    public const string LoungeButtonsScene006 = "event.lounge_buttons_scene_006"; 
    public const string LoungeButtonsScene007 = "event.lounge_buttons_scene_007"; 
    public const string LoungeButtonsScene008 = "event.lounge_buttons_scene_008"; 
    public const string FrontRoomLightScene001 = "event.front_room_light_scene_001"; 
    public const string FrontRoomLightScene002 = "event.front_room_light_scene_002"; 
    public const string FrontPorchLightEventValue = "event.front_porch_light_event_value"; 
    public const string FrontPorchLightScene001 = "event.front_porch_light_scene_001"; 
    public const string FrontPorchLightScene002 = "event.front_porch_light_scene_002"; 
    public const string EntryLightEventValue = "event.entry_light_event_value"; 
    public const string EntryLightScene001 = "event.entry_light_scene_001"; 
    public const string EntryLightScene002 = "event.entry_light_scene_002"; 
    public const string UpstairsHallEventValue = "event.upstairs_hall_event_value"; 
    public const string UpstairsHallScene001 = "event.upstairs_hall_scene_001"; 
    public const string UpstairsHallScene002 = "event.upstairs_hall_scene_002"; 
    public const string CraftRoomLightEventValue = "event.craft_room_light_event_value"; 
    public const string CraftRoomLightScene001 = "event.craft_room_light_scene_001"; 
    public const string CraftRoomLightScene002 = "event.craft_room_light_scene_002"; 
    public const string BackPorchLightScene001 = "event.back_porch_light_scene_001"; 
    public const string BackPorchLightScene002 = "event.back_porch_light_scene_002"; 
    public const string BackFloodScene001 = "event.back_flood_scene_001"; 
    public const string BackFloodScene002 = "event.back_flood_scene_002"; 
    public const string BasementStairLightScene001 = "event.basement_stair_light_scene_001"; 
    public const string BasementStairLightScene002 = "event.basement_stair_light_scene_002"; 
    public const string BasementStairLightScene003 = "event.basement_stair_light_scene_003"; 
    public const string BasementStairLightScene004 = "event.basement_stair_light_scene_004"; 
    public const string BasementStairLightScene005 = "event.basement_stair_light_scene_005"; 
    public const string BasementStairLightScene006 = "event.basement_stair_light_scene_006"; 
    public const string BasementLight3EventValue = "event.basement_light_3_event_value"; 
    public const string BasementLight3Scene001 = "event.basement_light_3_scene_001"; 
    public const string BasementLight3Scene002 = "event.basement_light_3_scene_002"; 
    public const string BasementLight1EventValue = "event.basement_light_1_event_value"; 
    public const string BasementLight1Scene001 = "event.basement_light_1_scene_001"; 
    public const string BasementLight1Scene002 = "event.basement_light_1_scene_002"; 
    public const string BasementLight2EventValue = "event.basement_light_2_event_value"; 
    public const string BasementLight2Scene001 = "event.basement_light_2_scene_001"; 
    public const string BasementLight2Scene002 = "event.basement_light_2_scene_002"; 
    public const string DoorbellRepeater579AVideoDoorbell = "event.doorbell_repeater_579a_video_doorbell";
}

public class Input_Boolean
{  
    public const string BedtimeSwitch = "input_boolean.bedtime_switch"; 
    public const string OfficeOverride = "input_boolean.office_override"; 
    public const string LivingRoomOverride = "input_boolean.living_room_override"; 
    public const string MaintenanceMode = "input_boolean.maintenance_mode"; 
    public const string BasementOverride = "input_boolean.basement_override"; 
    public const string DemoToggle1 = "input_boolean.demo_toggle_1"; 
    public const string DemoToggle2 = "input_boolean.demo_toggle_2";
}

public class Input_Button
{  
    public const string CustomMessageSend = "input_button.custom_message_send"; 
    public const string ClearNotifications = "input_button.clear_notifications"; 
    public const string PlayLastAudibleAlert = "input_button.play_last_audible_alert"; 
    public const string FindRokuRemote = "input_button.find_roku_remote"; 
    public const string AsherButton = "input_button.asher_button"; 
    public const string DemoButton1 = "input_button.demo_button_1"; 
    public const string DemoButton2 = "input_button.demo_button_2";
}

public class Input_Datetime
{  
    public const string TestDate = "input_datetime.test_date"; 
    public const string HakafkanetStartTime = "input_datetime.hakafkanet_start_time"; 
    public const string FilterLastChanged = "input_datetime.filter_last_changed"; 
    public const string FilterChangeRequired = "input_datetime.filter_change_required"; 
    public const string TestTimeOnly = "input_datetime.test_time_only"; 
    public const string LyraMedicine = "input_datetime.lyra_medicine";
}

public class Input_Number
{  
    public const string OfficeIlluminanceThreshold = "input_number.office_illuminance_threshold"; 
    public const string AudibleAlertToPlay = "input_number.audible_alert_to_play"; 
    public const string DemoTemperature = "input_number.demo_temperature"; 
    public const string DemoPowerMeter = "input_number.demo_power_meter"; 
    public const string DemoIlluminance = "input_number.demo_illuminance"; 
    public const string DemoBattery = "input_number.demo_battery";
}

public class Input_Text
{  
    public const string CustomMessage = "input_text.custom_message"; 
    public const string AudibleAlert1 = "input_text.audible_alert_1"; 
    public const string AudibleAlert2 = "input_text.audible_alert_2"; 
    public const string AudibleAlert3 = "input_text.audible_alert_3";
}

public class Light
{  
    public const string CouchOverhead = "light.couch_overhead"; 
    public const string TvBacklight = "light.tv_backlight"; 
    public const string OfficeLightGroup = "light.office_light_group"; 
    public const string MyLightGroup = "light.my_light_group"; 
    public const string BasementLightGroup = "light.basement_light_group"; 
    public const string MainBedroomOverhead = "light.main_bedroom_overhead"; 
    public const string OfficeCombinedLight = "light.office_combined_light"; 
    public const string DemoLight1 = "light.demo_light_1"; 
    public const string DemoLight2 = "light.demo_light_2"; 
    public const string LivingLamp1 = "light.living_lamp_1"; 
    public const string LoungeLights = "light.lounge_lights"; 
    public const string OfficeLightBars = "light.office_light_bars"; 
    public const string LivingLamp2 = "light.living_lamp_2"; 
    public const string WizRgbwTunable79A59C = "light.wiz_rgbw_tunable_79a59c"; 
    public const string WizRgbwTunable8D7D54 = "light.wiz_rgbw_tunable_8d7d54"; 
    public const string WizRgbwTunable79Aab4 = "light.wiz_rgbw_tunable_79aab4"; 
    public const string MainBedroomLight1 = "light.main_bedroom_light_1"; 
    public const string MainBedroomLight2 = "light.main_bedroom_light_2"; 
    public const string OfficeDisplayLights = "light.office_display_lights"; 
    public const string KitchenLights = "light.kitchen_lights"; 
    public const string DiningRoomLightsBasic = "light.dining_room_lights_basic"; 
    public const string FrontRoomLight = "light.front_room_light"; 
    public const string FrontPorchLight = "light.front_porch_light"; 
    public const string EntryLight = "light.entry_light"; 
    public const string UpstairsHall = "light.upstairs_hall"; 
    public const string CraftRoomLight = "light.craft_room_light"; 
    public const string BasementStairLightBasic = "light.basement_stair_light_basic"; 
    public const string BasementLight3 = "light.basement_light_3"; 
    public const string BasementLight1 = "light.basement_light_1"; 
    public const string BasementLight2 = "light.basement_light_2"; 
    public const string Garage1TiltBasic = "light.garage_1_tilt_basic"; 
    public const string Garage2TiltBasic = "light.garage_2_tilt_basic"; 
    public const string Garage1Opener = "light.garage_1_opener"; 
    public const string Garage2Opener = "light.garage_2_opener"; 
    public const string Rt149699InteriorLight = "light.rt149699_interior_light"; 
    public const string OfficeLedLight = "light.office_led_light"; 
    public const string MonkeyLight = "light.monkey_light";
}

public class Lock
{  
    public const string AqaraSmartLockU100 = "lock.aqara_smart_lock_u100"; 
    public const string _4S4Wmaed9L3477167DoorLocks = "lock.4s4wmaed9l3477167_door_locks";
}

public class Media_Player
{  
    public const string RokuUltra = "media_player.roku_ultra"; 
    public const string Xboxone = "media_player.xboxone"; 
    public const string SamsungQn90Aa65Tv2 = "media_player.samsung_qn90aa_65_tv_2"; 
    public const string SamsungQn90Aa65Tv = "media_player.samsung_qn90aa_65_tv"; 
    public const string SpotifyLeonardSperry = "media_player.spotify_leonard_sperry"; 
    public const string DiningRoomSpeaker = "media_player.dining_room_speaker"; 
    public const string AsherRoomSpeaker = "media_player.asher_room_speaker"; 
    public const string MainBedroomSpeaker = "media_player.main_bedroom_speaker";
}

public class Number
{  
    public const string WizRgbwTunable79A59CEffectSpeed = "number.wiz_rgbw_tunable_79a59c_effect_speed"; 
    public const string WizRgbwTunable8D7D54EffectSpeed = "number.wiz_rgbw_tunable_8d7d54_effect_speed"; 
    public const string WizRgbwTunable79Aab4EffectSpeed = "number.wiz_rgbw_tunable_79aab4_effect_speed"; 
    public const string MainBedroomLight1EffectSpeed = "number.main_bedroom_light_1_effect_speed"; 
    public const string MainBedroomLight2EffectSpeed = "number.main_bedroom_light_2_effect_speed"; 
    public const string EsphomeLivingRoomTimeout = "number.esphome_living_room_timeout"; 
    public const string EsphomeLivingRoomZone1X1 = "number.esphome_living_room_zone_1_x1"; 
    public const string EsphomeLivingRoomZone1Y1 = "number.esphome_living_room_zone_1_y1"; 
    public const string EsphomeLivingRoomZone1X2 = "number.esphome_living_room_zone_1_x2"; 
    public const string EsphomeLivingRoomZone1Y2 = "number.esphome_living_room_zone_1_y2"; 
    public const string EsphomeLivingRoomZone2X1 = "number.esphome_living_room_zone_2_x1"; 
    public const string EsphomeLivingRoomZone2Y1 = "number.esphome_living_room_zone_2_y1"; 
    public const string EsphomeLivingRoomZone2X2 = "number.esphome_living_room_zone_2_x2"; 
    public const string EsphomeLivingRoomZone2Y2 = "number.esphome_living_room_zone_2_y2"; 
    public const string EsphomeLivingRoomZone3X1 = "number.esphome_living_room_zone_3_x1"; 
    public const string EsphomeLivingRoomZone3Y1 = "number.esphome_living_room_zone_3_y1"; 
    public const string EsphomeLivingRoomZone3X2 = "number.esphome_living_room_zone_3_x2"; 
    public const string EsphomeLivingRoomZone3Y2 = "number.esphome_living_room_zone_3_y2"; 
    public const string EsphomekitchenmotionTimeout = "number.esphomekitchenmotion_timeout"; 
    public const string EsphomekitchenmotionZone1X1 = "number.esphomekitchenmotion_zone_1_x1"; 
    public const string EsphomekitchenmotionZone1Y1 = "number.esphomekitchenmotion_zone_1_y1"; 
    public const string EsphomekitchenmotionZone1X2 = "number.esphomekitchenmotion_zone_1_x2"; 
    public const string EsphomekitchenmotionZone1Y2 = "number.esphomekitchenmotion_zone_1_y2"; 
    public const string EsphomekitchenmotionZone2X1 = "number.esphomekitchenmotion_zone_2_x1"; 
    public const string EsphomekitchenmotionZone2Y1 = "number.esphomekitchenmotion_zone_2_y1"; 
    public const string EsphomekitchenmotionZone2X2 = "number.esphomekitchenmotion_zone_2_x2"; 
    public const string EsphomekitchenmotionZone2Y2 = "number.esphomekitchenmotion_zone_2_y2"; 
    public const string EsphomekitchenmotionZone3X1 = "number.esphomekitchenmotion_zone_3_x1"; 
    public const string EsphomekitchenmotionZone3Y1 = "number.esphomekitchenmotion_zone_3_y1"; 
    public const string EsphomekitchenmotionZone3X2 = "number.esphomekitchenmotion_zone_3_x2"; 
    public const string EsphomekitchenmotionZone3Y2 = "number.esphomekitchenmotion_zone_3_y2"; 
    public const string BackHallLightIndicatorValue = "number.back_hall_light_indicator_value"; 
    public const string DiningRoomLightsIndicatorValue = "number.dining_room_lights_indicator_value"; 
    public const string PeacockLampIndicatorValue = "number.peacock_lamp_indicator_value"; 
    public const string BackPorchLightIndicatorValue = "number.back_porch_light_indicator_value"; 
    public const string BackFloodIndicatorValue = "number.back_flood_indicator_value"; 
    public const string BasementStairLightIndicatorValue = "number.basement_stair_light_indicator_value"; 
    public const string KazulTempHumidityIndicatorValue = "number.kazul_temp_humidity_indicator_value"; 
    public const string _4In1SensorIndicatorValue = "number.4_in_1_sensor_indicator_value"; 
    public const string GarageDoorOpenerRelay1AutoTurnOffTimer = "number.garage_door_opener_relay_1_auto_turn_off_timer"; 
    public const string PlantPlug1IndicatorValue = "number.plant_plug_1_indicator_value"; 
    public const string MbrDresserPlugIndicatorValue = "number.mbr_dresser_plug_indicator_value"; 
    public const string OutsideDualPlugIndicatorValue = "number.outside_dual_plug_indicator_value"; 
    public const string DiningRoomSpeakerBass = "number.dining_room_speaker_bass"; 
    public const string DiningRoomSpeakerBalance = "number.dining_room_speaker_balance"; 
    public const string DiningRoomSpeakerTreble = "number.dining_room_speaker_treble"; 
    public const string AsherRoomSpeakerBass = "number.asher_room_speaker_bass"; 
    public const string AsherRoomSpeakerBalance = "number.asher_room_speaker_balance"; 
    public const string AsherRoomSpeakerTreble = "number.asher_room_speaker_treble"; 
    public const string MainBedroomSpeakerBass = "number.main_bedroom_speaker_bass"; 
    public const string MainBedroomSpeakerBalance = "number.main_bedroom_speaker_balance"; 
    public const string MainBedroomSpeakerTreble = "number.main_bedroom_speaker_treble"; 
    public const string OfficeLedOnOffTransitionTime = "number.office_led_on_off_transition_time"; 
    public const string OfficeLedStartUpCurrentLevel = "number.office_led_start_up_current_level"; 
    public const string OfficeLedStartUpColorTemperature = "number.office_led_start_up_color_temperature"; 
    public const string MbrMotion1DetectionInterval = "number.mbr_motion_1_detection_interval"; 
    public const string MbrMotion2DetectionInterval = "number.mbr_motion_2_detection_interval"; 
    public const string BasementMotion2DetectionInterval = "number.basement_motion_2_detection_interval";
}

public class Person
{  
    public const string Leonard = "person.leonard"; 
    public const string Rachel = "person.rachel"; 
    public const string TheValkyrie = "person.the_valkyrie";
}

public class Remote
{  
    public const string RokuUltra = "remote.roku_ultra"; 
    public const string SamsungQn90Aa65Tv = "remote.samsung_qn90aa_65_tv";
}

public class Schedule
{  
    public const string TestSchedule = "schedule.test_schedule"; 
    public const string PeriodicSchedule = "schedule.periodic_schedule";
}

public class Script
{  
    public const string Weatheralerts1PopupOnWxAlert = "script.weatheralerts_1_popup_on_wx_alert";
}

public class Select
{  
    public const string MbrDadSideSwitchPowerOnBehaviorOnStartup = "select.mbr_dad_side_switch_power_on_behavior_on_startup"; 
    public const string FrontRoomComputerLampPowerOnBehaviorOnStartup = "select.front_room_computer_lamp_power_on_behavior_on_startup"; 
    public const string EsphomeLivingRoomBaudRate = "select.esphome_living_room_baud_rate"; 
    public const string EsphomeLivingRoomZoneType = "select.esphome_living_room_zone_type"; 
    public const string EsphomekitchenmotionBaudRate = "select.esphomekitchenmotion_baud_rate"; 
    public const string EsphomekitchenmotionZoneType = "select.esphomekitchenmotion_zone_type"; 
    public const string OfficeDisplayLightsSceneControl = "select.office_display_lights_scene_control"; 
    public const string BackHallLightSceneControl = "select.back_hall_light_scene_control"; 
    public const string DiningRoomLightsLedIndicatorColorButton1 = "select.dining_room_lights_led_indicator_color_button_1"; 
    public const string DiningRoomLightsLedIndicatorColorButton2 = "select.dining_room_lights_led_indicator_color_button_2"; 
    public const string DiningRoomLightsLedIndicatorColorButton3 = "select.dining_room_lights_led_indicator_color_button_3"; 
    public const string DiningRoomLightsLedIndicatorColorButton4 = "select.dining_room_lights_led_indicator_color_button_4"; 
    public const string GarageDoorOpenerSwitch1Type = "select.garage_door_opener_switch_1_type"; 
    public const string GarageDoorOpenerSwitch2Type = "select.garage_door_opener_switch_2_type"; 
    public const string GarageDoorOpenerRelay1AutoTurnOffTimerUnit = "select.garage_door_opener_relay_1_auto_turn_off_timer_unit"; 
    public const string Rt149699ConvertableDrawerMode = "select.rt149699_convertable_drawer_mode"; 
    public const string OfficeLedStartUpBehavior = "select.office_led_start_up_behavior";
}

public class Sensor
{  
    public const string SunNextDawn = "sensor.sun_next_dawn"; 
    public const string SunNextDusk = "sensor.sun_next_dusk"; 
    public const string SunNextMidnight = "sensor.sun_next_midnight"; 
    public const string SunNextNoon = "sensor.sun_next_noon"; 
    public const string SunNextRising = "sensor.sun_next_rising"; 
    public const string SunNextSetting = "sensor.sun_next_setting"; 
    public const string OfficeIlluminanceStatistics = "sensor.office_illuminance_statistics"; 
    public const string AqaraSmartLockU100Battery = "sensor.aqara_smart_lock_u100_battery"; 
    public const string AqaraSmartLockU100Voltage = "sensor.aqara_smart_lock_u100_voltage"; 
    public const string MfcL8900CdwStatus = "sensor.mfc_l8900cdw_status"; 
    public const string MfcL8900CdwPageCounter = "sensor.mfc_l8900cdw_page_counter"; 
    public const string MfcL8900CdwBWPages = "sensor.mfc_l8900cdw_b_w_pages"; 
    public const string MfcL8900CdwColorPages = "sensor.mfc_l8900cdw_color_pages"; 
    public const string MfcL8900CdwDuplexUnitPageCounter = "sensor.mfc_l8900cdw_duplex_unit_page_counter"; 
    public const string MfcL8900CdwDrumRemainingLifetime = "sensor.mfc_l8900cdw_drum_remaining_lifetime"; 
    public const string MfcL8900CdwDrumRemainingPages = "sensor.mfc_l8900cdw_drum_remaining_pages"; 
    public const string MfcL8900CdwDrumPageCounter = "sensor.mfc_l8900cdw_drum_page_counter"; 
    public const string MfcL8900CdwBeltUnitRemainingLifetime = "sensor.mfc_l8900cdw_belt_unit_remaining_lifetime"; 
    public const string MfcL8900CdwFuserRemainingLifetime = "sensor.mfc_l8900cdw_fuser_remaining_lifetime"; 
    public const string MfcL8900CdwLaserRemainingLifetime = "sensor.mfc_l8900cdw_laser_remaining_lifetime"; 
    public const string MfcL8900CdwPfKit1RemainingLifetime = "sensor.mfc_l8900cdw_pf_kit_1_remaining_lifetime"; 
    public const string MfcL8900CdwPfKitMpRemainingLifetime = "sensor.mfc_l8900cdw_pf_kit_mp_remaining_lifetime"; 
    public const string MfcL8900CdwBlackTonerRemaining = "sensor.mfc_l8900cdw_black_toner_remaining"; 
    public const string MfcL8900CdwCyanTonerRemaining = "sensor.mfc_l8900cdw_cyan_toner_remaining"; 
    public const string MfcL8900CdwMagentaTonerRemaining = "sensor.mfc_l8900cdw_magenta_toner_remaining"; 
    public const string MfcL8900CdwYellowTonerRemaining = "sensor.mfc_l8900cdw_yellow_toner_remaining"; 
    public const string UswFlexMiniUptime = "sensor.usw_flex_mini_uptime"; 
    public const string PicardUptime = "sensor.picard_uptime"; 
    public const string HeimdallUptime = "sensor.heimdall_uptime"; 
    public const string PicardTemperature = "sensor.picard_temperature"; 
    public const string SuluUplinkMac = "sensor.sulu_uplink_mac"; 
    public const string PicardUplinkMac = "sensor.picard_uplink_mac"; 
    public const string UswFlexMiniState = "sensor.usw_flex_mini_state"; 
    public const string PicardState = "sensor.picard_state"; 
    public const string HeimdallState = "sensor.heimdall_state"; 
    public const string SuluCpuUtilization = "sensor.sulu_cpu_utilization"; 
    public const string PicardCpuUtilization = "sensor.picard_cpu_utilization"; 
    public const string HeimdallCpuUtilization = "sensor.heimdall_cpu_utilization"; 
    public const string SuluMemoryUtilization = "sensor.sulu_memory_utilization"; 
    public const string PicardMemoryUtilization = "sensor.picard_memory_utilization"; 
    public const string HeimdallMemoryUtilization = "sensor.heimdall_memory_utilization"; 
    public const string HeimdallMicrosoftWanLatency = "sensor.heimdall_microsoft_wan_latency"; 
    public const string HeimdallGoogleWanLatency = "sensor.heimdall_google_wan_latency"; 
    public const string HeimdallCloudflareWanLatency = "sensor.heimdall_cloudflare_wan_latency"; 
    public const string HeimdallMicrosoftWan2Latency = "sensor.heimdall_microsoft_wan2_latency"; 
    public const string HeimdallGoogleWan2Latency = "sensor.heimdall_google_wan2_latency"; 
    public const string HeimdallCloudflareWan2Latency = "sensor.heimdall_cloudflare_wan2_latency"; 
    public const string RachelPhoneBatteryLevel = "sensor.rachel_phone_battery_level"; 
    public const string RachelPhoneBatteryState = "sensor.rachel_phone_battery_state"; 
    public const string RachelPhoneChargerType = "sensor.rachel_phone_charger_type"; 
    public const string LeonardPhoneBatteryLevel = "sensor.leonard_phone_battery_level"; 
    public const string LeonardPhoneBatteryState = "sensor.leonard_phone_battery_state"; 
    public const string LeonardPhoneChargerType = "sensor.leonard_phone_charger_type"; 
    public const string Livingroomandkitchenpresencecount = "sensor.livingroomandkitchenpresencecount"; 
    public const string LeosperryHaKafkaNetDiscussions = "sensor.leosperry_ha_kafka_net_discussions"; 
    public const string LeosperryHaKafkaNetStars = "sensor.leosperry_ha_kafka_net_stars"; 
    public const string LeosperryHaKafkaNetWatchers = "sensor.leosperry_ha_kafka_net_watchers"; 
    public const string LeosperryHaKafkaNetForks = "sensor.leosperry_ha_kafka_net_forks"; 
    public const string LeosperryHaKafkaNetIssues = "sensor.leosperry_ha_kafka_net_issues"; 
    public const string LeosperryHaKafkaNetPullRequests = "sensor.leosperry_ha_kafka_net_pull_requests"; 
    public const string LeosperryHaKafkaNetLatestCommit = "sensor.leosperry_ha_kafka_net_latest_commit"; 
    public const string LeosperryHaKafkaNetLatestDiscussion = "sensor.leosperry_ha_kafka_net_latest_discussion"; 
    public const string LeosperryHaKafkaNetLatestRelease = "sensor.leosperry_ha_kafka_net_latest_release"; 
    public const string LeosperryHaKafkaNetLatestIssue = "sensor.leosperry_ha_kafka_net_latest_issue"; 
    public const string LeosperryHaKafkaNetLatestPullRequest = "sensor.leosperry_ha_kafka_net_latest_pull_request"; 
    public const string LeosperryHaKafkaNetLatestTag = "sensor.leosperry_ha_kafka_net_latest_tag"; 
    public const string NwsAlerts2 = "sensor.nws_alerts_2"; 
    public const string NwsAlerts = "sensor.nws_alerts"; 
    public const string RokuUltraActiveApp = "sensor.roku_ultra_active_app"; 
    public const string RokuUltraActiveAppId = "sensor.roku_ultra_active_app_id"; 
    public const string BrotherMfcL8900CdwSeriesBk = "sensor.brother_mfc_l8900cdw_series_bk"; 
    public const string BrotherMfcL8900CdwSeriesC = "sensor.brother_mfc_l8900cdw_series_c"; 
    public const string BrotherMfcL8900CdwSeriesM = "sensor.brother_mfc_l8900cdw_series_m"; 
    public const string BrotherMfcL8900CdwSeriesY = "sensor.brother_mfc_l8900cdw_series_y"; 
    public const string BrotherMfcL8900CdwSeries = "sensor.brother_mfc_l8900cdw_series"; 
    public const string SolaredgeLifetimeEnergy = "sensor.solaredge_lifetime_energy"; 
    public const string SolaredgeCurrentPower = "sensor.solaredge_current_power"; 
    public const string SolaredgePowerConsumption = "sensor.solaredge_power_consumption"; 
    public const string SolaredgeSolarPower = "sensor.solaredge_solar_power"; 
    public const string SolaredgeGridPower = "sensor.solaredge_grid_power"; 
    public const string SolaredgeImportedEnergy = "sensor.solaredge_imported_energy"; 
    public const string SolaredgeProductionEnergy = "sensor.solaredge_production_energy"; 
    public const string SolaredgeConsumptionEnergy = "sensor.solaredge_consumption_energy"; 
    public const string SolaredgeSelfconsumptionEnergy = "sensor.solaredge_selfconsumption_energy"; 
    public const string SolaredgeExportedEnergy = "sensor.solaredge_exported_energy"; 
    public const string WatchmanLastUpdated = "sensor.watchman_last_updated"; 
    public const string WatchmanMissingEntities = "sensor.watchman_missing_entities"; 
    public const string WatchmanMissingServices = "sensor.watchman_missing_services"; 
    public const string KthvTemperature = "sensor.kthv_temperature"; 
    public const string KthvWindChill = "sensor.kthv_wind_chill"; 
    public const string KthvRelativeHumidity = "sensor.kthv_relative_humidity"; 
    public const string KthvWindSpeed = "sensor.kthv_wind_speed"; 
    public const string KthvWindGust = "sensor.kthv_wind_gust"; 
    public const string KthvWindDirection = "sensor.kthv_wind_direction"; 
    public const string KthvBarometricPressure = "sensor.kthv_barometric_pressure"; 
    public const string KthvLatestObservationTime = "sensor.kthv_latest_observation_time"; 
    public const string ElectricityMapsCo2Intensity = "sensor.electricity_maps_co2_intensity"; 
    public const string ElectricityMapsGridFossilFuelPercentage = "sensor.electricity_maps_grid_fossil_fuel_percentage"; 
    public const string WizRgbwTunable79A59CPower = "sensor.wiz_rgbw_tunable_79a59c_power"; 
    public const string WizRgbwTunable8D7D54Power = "sensor.wiz_rgbw_tunable_8d7d54_power"; 
    public const string WizRgbwTunable79Aab4Power = "sensor.wiz_rgbw_tunable_79aab4_power"; 
    public const string MainBedroomLight1Power = "sensor.main_bedroom_light_1_power"; 
    public const string MainBedroomLight2Power = "sensor.main_bedroom_light_2_power"; 
    public const string GlancesSperryRocksEtcResolvConfDiskUsed = "sensor.glances_sperry_rocks_etc_resolv_conf_disk_used"; 
    public const string GlancesSperryRocksEtcResolvConfDiskUsage = "sensor.glances_sperry_rocks_etc_resolv_conf_disk_usage"; 
    public const string GlancesSperryRocksEtcResolvConfDiskFree = "sensor.glances_sperry_rocks_etc_resolv_conf_disk_free"; 
    public const string GlancesSperryRocksEtcHostnameDiskUsed = "sensor.glances_sperry_rocks_etc_hostname_disk_used"; 
    public const string GlancesSperryRocksEtcHostnameDiskUsage = "sensor.glances_sperry_rocks_etc_hostname_disk_usage"; 
    public const string GlancesSperryRocksEtcHostnameDiskFree = "sensor.glances_sperry_rocks_etc_hostname_disk_free"; 
    public const string GlancesSperryRocksEtcHostsDiskUsed = "sensor.glances_sperry_rocks_etc_hosts_disk_used"; 
    public const string GlancesSperryRocksEtcHostsDiskUsage = "sensor.glances_sperry_rocks_etc_hosts_disk_usage"; 
    public const string GlancesSperryRocksEtcHostsDiskFree = "sensor.glances_sperry_rocks_etc_hosts_disk_free"; 
    public const string GlancesSperryRocksCompositeTemperature = "sensor.glances_sperry_rocks_composite_temperature"; 
    public const string GlancesSperryRocksCore0Temperature = "sensor.glances_sperry_rocks_core_0_temperature"; 
    public const string GlancesSperryRocksCore1Temperature = "sensor.glances_sperry_rocks_core_1_temperature"; 
    public const string GlancesSperryRocksCore2Temperature = "sensor.glances_sperry_rocks_core_2_temperature"; 
    public const string GlancesSperryRocksCore3Temperature = "sensor.glances_sperry_rocks_core_3_temperature"; 
    public const string GlancesSperryRocksPackageId0Temperature = "sensor.glances_sperry_rocks_package_id_0_temperature"; 
    public const string GlancesSperryRocksSensor1Temperature = "sensor.glances_sperry_rocks_sensor_1_temperature"; 
    public const string GlancesSperryRocksSensor2Temperature = "sensor.glances_sperry_rocks_sensor_2_temperature"; 
    public const string GlancesSperryRocksSensor3Temperature = "sensor.glances_sperry_rocks_sensor_3_temperature"; 
    public const string GlancesSperryRocksSensor4Temperature = "sensor.glances_sperry_rocks_sensor_4_temperature"; 
    public const string GlancesSperryRocksSensor5Temperature = "sensor.glances_sperry_rocks_sensor_5_temperature"; 
    public const string GlancesSperryRocksSensor6Temperature = "sensor.glances_sperry_rocks_sensor_6_temperature"; 
    public const string GlancesSperryRocksSensor7Temperature = "sensor.glances_sperry_rocks_sensor_7_temperature"; 
    public const string GlancesSperryRocksSensor8Temperature = "sensor.glances_sperry_rocks_sensor_8_temperature"; 
    public const string GlancesSperryRocksAcpitz0Temperature = "sensor.glances_sperry_rocks_acpitz_0_temperature"; 
    public const string GlancesSperryRocksBatteryCharge = "sensor.glances_sperry_rocks_battery_charge"; 
    public const string GlancesSperryRocksMemoryUsage = "sensor.glances_sperry_rocks_memory_usage"; 
    public const string GlancesSperryRocksMemoryUse = "sensor.glances_sperry_rocks_memory_use"; 
    public const string GlancesSperryRocksMemoryFree = "sensor.glances_sperry_rocks_memory_free"; 
    public const string GlancesSperryRocksSwapUsage = "sensor.glances_sperry_rocks_swap_usage"; 
    public const string GlancesSperryRocksSwapUse = "sensor.glances_sperry_rocks_swap_use"; 
    public const string GlancesSperryRocksSwapFree = "sensor.glances_sperry_rocks_swap_free"; 
    public const string GlancesSperryRocksCpuLoad = "sensor.glances_sperry_rocks_cpu_load"; 
    public const string GlancesSperryRocksRunning = "sensor.glances_sperry_rocks_running"; 
    public const string GlancesSperryRocksTotal = "sensor.glances_sperry_rocks_total"; 
    public const string GlancesSperryRocksThreads = "sensor.glances_sperry_rocks_threads"; 
    public const string GlancesSperryRocksSleeping = "sensor.glances_sperry_rocks_sleeping"; 
    public const string GlancesSperryRocksCpuUsage = "sensor.glances_sperry_rocks_cpu_usage"; 
    public const string GlancesSperryRocksLoRx = "sensor.glances_sperry_rocks_lo_rx"; 
    public const string GlancesSperryRocksLoTx = "sensor.glances_sperry_rocks_lo_tx"; 
    public const string GlancesSperryRocksEth0Rx = "sensor.glances_sperry_rocks_eth0_rx"; 
    public const string GlancesSperryRocksEth0Tx = "sensor.glances_sperry_rocks_eth0_tx"; 
    public const string GlancesSperryRocksContainersActive = "sensor.glances_sperry_rocks_containers_active"; 
    public const string GlancesSperryRocksContainersCpuUsage = "sensor.glances_sperry_rocks_containers_cpu_usage"; 
    public const string GlancesSperryRocksContainersMemoryUsed = "sensor.glances_sperry_rocks_containers_memory_used"; 
    public const string GlancesSperryRocksNvme0N1DiskRead = "sensor.glances_sperry_rocks_nvme0n1_disk_read"; 
    public const string GlancesSperryRocksNvme0N1DiskWrite = "sensor.glances_sperry_rocks_nvme0n1_disk_write"; 
    public const string GlancesSperryRocksNvme0N1P1DiskRead = "sensor.glances_sperry_rocks_nvme0n1p1_disk_read"; 
    public const string GlancesSperryRocksNvme0N1P1DiskWrite = "sensor.glances_sperry_rocks_nvme0n1p1_disk_write"; 
    public const string GlancesSperryRocksNvme0N1P2DiskRead = "sensor.glances_sperry_rocks_nvme0n1p2_disk_read"; 
    public const string GlancesSperryRocksNvme0N1P2DiskWrite = "sensor.glances_sperry_rocks_nvme0n1p2_disk_write"; 
    public const string GlancesSperryRocksDm0DiskRead = "sensor.glances_sperry_rocks_dm_0_disk_read"; 
    public const string GlancesSperryRocksDm0DiskWrite = "sensor.glances_sperry_rocks_dm_0_disk_write"; 
    public const string GlancesSperryRocksDm1DiskRead = "sensor.glances_sperry_rocks_dm_1_disk_read"; 
    public const string GlancesSperryRocksDm1DiskWrite = "sensor.glances_sperry_rocks_dm_1_disk_write"; 
    public const string GlancesSperryRocksUptime = "sensor.glances_sperry_rocks_uptime"; 
    public const string LocalhostDataDiskUsed = "sensor.localhost_data_disk_used"; 
    public const string LocalhostDataDiskUsage = "sensor.localhost_data_disk_usage"; 
    public const string LocalhostDataDiskFree = "sensor.localhost_data_disk_free"; 
    public const string LocalhostCpuThermal0Temperature = "sensor.localhost_cpu_thermal_0_temperature"; 
    public const string LocalhostCompositeTemperature = "sensor.localhost_composite_temperature"; 
    public const string LocalhostSensor1Temperature = "sensor.localhost_sensor_1_temperature"; 
    public const string LocalhostSensor2Temperature = "sensor.localhost_sensor_2_temperature"; 
    public const string LocalhostMemoryUsage = "sensor.localhost_memory_usage"; 
    public const string LocalhostMemoryUse = "sensor.localhost_memory_use"; 
    public const string LocalhostMemoryFree = "sensor.localhost_memory_free"; 
    public const string LocalhostSwapUsage = "sensor.localhost_swap_usage"; 
    public const string LocalhostSwapUse = "sensor.localhost_swap_use"; 
    public const string LocalhostSwapFree = "sensor.localhost_swap_free"; 
    public const string LocalhostCpuLoad = "sensor.localhost_cpu_load"; 
    public const string LocalhostRunning = "sensor.localhost_running"; 
    public const string LocalhostTotal = "sensor.localhost_total"; 
    public const string LocalhostThreads = "sensor.localhost_threads"; 
    public const string LocalhostSleeping = "sensor.localhost_sleeping"; 
    public const string LocalhostCpuUsage = "sensor.localhost_cpu_usage"; 
    public const string LocalhostLoRx = "sensor.localhost_lo_rx"; 
    public const string LocalhostLoTx = "sensor.localhost_lo_tx"; 
    public const string LocalhostEnd0Rx = "sensor.localhost_end0_rx"; 
    public const string LocalhostEnd0Tx = "sensor.localhost_end0_tx"; 
    public const string LocalhostWlan0Rx = "sensor.localhost_wlan0_rx"; 
    public const string LocalhostWlan0Tx = "sensor.localhost_wlan0_tx"; 
    public const string LocalhostDocker0Rx = "sensor.localhost_docker0_rx"; 
    public const string LocalhostDocker0Tx = "sensor.localhost_docker0_tx"; 
    public const string LocalhostHassioRx = "sensor.localhost_hassio_rx"; 
    public const string LocalhostHassioTx = "sensor.localhost_hassio_tx"; 
    public const string LocalhostContainersActive = "sensor.localhost_containers_active"; 
    public const string LocalhostContainersCpuUsage = "sensor.localhost_containers_cpu_usage"; 
    public const string LocalhostContainersMemoryUsed = "sensor.localhost_containers_memory_used"; 
    public const string LocalhostZram0DiskRead = "sensor.localhost_zram0_disk_read"; 
    public const string LocalhostZram0DiskWrite = "sensor.localhost_zram0_disk_write"; 
    public const string LocalhostZram1DiskRead = "sensor.localhost_zram1_disk_read"; 
    public const string LocalhostZram1DiskWrite = "sensor.localhost_zram1_disk_write"; 
    public const string LocalhostZram2DiskRead = "sensor.localhost_zram2_disk_read"; 
    public const string LocalhostZram2DiskWrite = "sensor.localhost_zram2_disk_write"; 
    public const string LocalhostNvme0N1DiskRead = "sensor.localhost_nvme0n1_disk_read"; 
    public const string LocalhostNvme0N1DiskWrite = "sensor.localhost_nvme0n1_disk_write"; 
    public const string LocalhostNvme0N1P1DiskRead = "sensor.localhost_nvme0n1p1_disk_read"; 
    public const string LocalhostNvme0N1P1DiskWrite = "sensor.localhost_nvme0n1p1_disk_write"; 
    public const string LocalhostMmcblk0DiskRead = "sensor.localhost_mmcblk0_disk_read"; 
    public const string LocalhostMmcblk0DiskWrite = "sensor.localhost_mmcblk0_disk_write"; 
    public const string LocalhostMmcblk0P1DiskRead = "sensor.localhost_mmcblk0p1_disk_read"; 
    public const string LocalhostMmcblk0P1DiskWrite = "sensor.localhost_mmcblk0p1_disk_write"; 
    public const string LocalhostMmcblk0P2DiskRead = "sensor.localhost_mmcblk0p2_disk_read"; 
    public const string LocalhostMmcblk0P2DiskWrite = "sensor.localhost_mmcblk0p2_disk_write"; 
    public const string LocalhostMmcblk0P3DiskRead = "sensor.localhost_mmcblk0p3_disk_read"; 
    public const string LocalhostMmcblk0P3DiskWrite = "sensor.localhost_mmcblk0p3_disk_write"; 
    public const string LocalhostMmcblk0P4DiskRead = "sensor.localhost_mmcblk0p4_disk_read"; 
    public const string LocalhostMmcblk0P4DiskWrite = "sensor.localhost_mmcblk0p4_disk_write"; 
    public const string LocalhostMmcblk0P5DiskRead = "sensor.localhost_mmcblk0p5_disk_read"; 
    public const string LocalhostMmcblk0P5DiskWrite = "sensor.localhost_mmcblk0p5_disk_write"; 
    public const string LocalhostMmcblk0P6DiskRead = "sensor.localhost_mmcblk0p6_disk_read"; 
    public const string LocalhostMmcblk0P6DiskWrite = "sensor.localhost_mmcblk0p6_disk_write"; 
    public const string LocalhostMmcblk0P7DiskRead = "sensor.localhost_mmcblk0p7_disk_read"; 
    public const string LocalhostMmcblk0P7DiskWrite = "sensor.localhost_mmcblk0p7_disk_write"; 
    public const string LocalhostMmcblk0P8DiskRead = "sensor.localhost_mmcblk0p8_disk_read"; 
    public const string LocalhostMmcblk0P8DiskWrite = "sensor.localhost_mmcblk0p8_disk_write"; 
    public const string LocalhostMmcblk0Boot0DiskRead = "sensor.localhost_mmcblk0boot0_disk_read"; 
    public const string LocalhostMmcblk0Boot0DiskWrite = "sensor.localhost_mmcblk0boot0_disk_write"; 
    public const string LocalhostMmcblk0Boot1DiskRead = "sensor.localhost_mmcblk0boot1_disk_read"; 
    public const string LocalhostMmcblk0Boot1DiskWrite = "sensor.localhost_mmcblk0boot1_disk_write"; 
    public const string LocalhostUptime = "sensor.localhost_uptime"; 
    public const string UsbControllerStatus = "sensor.usb_controller_status"; 
    public const string Node2NodeStatus = "sensor.node_2_node_status"; 
    public const string Node3NodeStatus = "sensor.node_3_node_status"; 
    public const string Node4NodeStatus = "sensor.node_4_node_status"; 
    public const string Node5NodeStatus = "sensor.node_5_node_status"; 
    public const string Node6NodeStatus = "sensor.node_6_node_status"; 
    public const string Node7NodeStatus = "sensor.node_7_node_status"; 
    public const string Node8NodeStatus = "sensor.node_8_node_status"; 
    public const string Node10NodeStatus = "sensor.node_10_node_status"; 
    public const string Node12NodeStatus = "sensor.node_12_node_status"; 
    public const string Node14NodeStatus = "sensor.node_14_node_status"; 
    public const string Node15NodeStatus = "sensor.node_15_node_status"; 
    public const string Node16NodeStatus = "sensor.node_16_node_status"; 
    public const string Node17NodeStatus = "sensor.node_17_node_status"; 
    public const string Node18NodeStatus = "sensor.node_18_node_status"; 
    public const string Node20NodeStatus = "sensor.node_20_node_status"; 
    public const string Node21NodeStatus = "sensor.node_21_node_status"; 
    public const string Node22NodeStatus = "sensor.node_22_node_status"; 
    public const string Node23NodeStatus = "sensor.node_23_node_status"; 
    public const string Node24NodeStatus = "sensor.node_24_node_status"; 
    public const string Node30NodeStatus = "sensor.node_30_node_status"; 
    public const string Node31NodeStatus = "sensor.node_31_node_status"; 
    public const string Node32NodeStatus = "sensor.node_32_node_status"; 
    public const string Node33NodeStatus = "sensor.node_33_node_status"; 
    public const string Node34NodeStatus = "sensor.node_34_node_status"; 
    public const string Node35NodeStatus = "sensor.node_35_node_status"; 
    public const string Node38NodeStatus = "sensor.node_38_node_status"; 
    public const string Node39NodeStatus = "sensor.node_39_node_status"; 
    public const string OutsideDualPlugNodeStatus = "sensor.outside_dual_plug_node_status"; 
    public const string _4S4Wmaed9L3477167Odometer = "sensor.4s4wmaed9l3477167_odometer"; 
    public const string _4S4Wmaed9L3477167AverageFuelConsumption = "sensor.4s4wmaed9l3477167_average_fuel_consumption"; 
    public const string _4S4Wmaed9L3477167Range = "sensor.4s4wmaed9l3477167_range"; 
    public const string _4S4Wmaed9L3477167TirePressureFrontLeft = "sensor.4s4wmaed9l3477167_tire_pressure_front_left"; 
    public const string _4S4Wmaed9L3477167TirePressureFrontRight = "sensor.4s4wmaed9l3477167_tire_pressure_front_right"; 
    public const string _4S4Wmaed9L3477167TirePressureRearLeft = "sensor.4s4wmaed9l3477167_tire_pressure_rear_left"; 
    public const string _4S4Wmaed9L3477167TirePressureRearRight = "sensor.4s4wmaed9l3477167_tire_pressure_rear_right"; 
    public const string DiskUsePercent = "sensor.disk_use_percent"; 
    public const string MemoryUsePercent = "sensor.memory_use_percent"; 
    public const string ProcessorUse = "sensor.processor_use"; 
    public const string ProcessorTemperature = "sensor.processor_temperature"; 
    public const string SwapUsePercent = "sensor.swap_use_percent"; 
    public const string EsphomeLivingRoomWifiSignalSensor = "sensor.esphome_living_room_wifi_signal_sensor"; 
    public const string EsphomeLivingRoomPresenceTargetCount = "sensor.esphome_living_room_presence_target_count"; 
    public const string EsphomeLivingRoomStillTargetCount = "sensor.esphome_living_room_still_target_count"; 
    public const string EsphomeLivingRoomMovingTargetCount = "sensor.esphome_living_room_moving_target_count"; 
    public const string EsphomeLivingRoomZone1AllTargetCount = "sensor.esphome_living_room_zone_1_all_target_count"; 
    public const string EsphomeLivingRoomZone1StillTargetCount = "sensor.esphome_living_room_zone_1_still_target_count"; 
    public const string EsphomeLivingRoomZone1MovingTargetCount = "sensor.esphome_living_room_zone_1_moving_target_count"; 
    public const string EsphomeLivingRoomZone2AllTargetCount = "sensor.esphome_living_room_zone_2_all_target_count"; 
    public const string EsphomeLivingRoomZone2StillTargetCount = "sensor.esphome_living_room_zone_2_still_target_count"; 
    public const string EsphomeLivingRoomZone2MovingTargetCount = "sensor.esphome_living_room_zone_2_moving_target_count"; 
    public const string EsphomeLivingRoomZone3AllTargetCount = "sensor.esphome_living_room_zone_3_all_target_count"; 
    public const string EsphomeLivingRoomZone3StillTargetCount = "sensor.esphome_living_room_zone_3_still_target_count"; 
    public const string EsphomeLivingRoomZone3MovingTargetCount = "sensor.esphome_living_room_zone_3_moving_target_count"; 
    public const string EsphomeLivingRoomConnectedSsid = "sensor.esphome_living_room_connected_ssid"; 
    public const string EsphomeLivingRoomMacWifiAddress = "sensor.esphome_living_room_mac_wifi_address"; 
    public const string EsphomeLivingRoomIpAddress = "sensor.esphome_living_room_ip_address"; 
    public const string EsphomeLivingRoomEsphomeVersion = "sensor.esphome_living_room_esphome_version"; 
    public const string EsphomeLivingRoomUptimeHumanReadable = "sensor.esphome_living_room_uptime_human_readable"; 
    public const string EsphomeLivingRoomLd2450Firmware = "sensor.esphome_living_room_ld2450_firmware"; 
    public const string EsphomeLivingRoomLd2450BtMac = "sensor.esphome_living_room_ld2450_bt_mac"; 
    public const string EsphomekitchenmotionWifiSignalSensor = "sensor.esphomekitchenmotion_wifi_signal_sensor"; 
    public const string EsphomekitchenmotionPresenceTargetCount = "sensor.esphomekitchenmotion_presence_target_count"; 
    public const string EsphomekitchenmotionZone1AllTargetCount = "sensor.esphomekitchenmotion_zone_1_all_target_count"; 
    public const string EsphomekitchenmotionZone2AllTargetCount = "sensor.esphomekitchenmotion_zone_2_all_target_count"; 
    public const string EsphomekitchenmotionConnectedSsid = "sensor.esphomekitchenmotion_connected_ssid"; 
    public const string EsphomekitchenmotionMacWifiAddress = "sensor.esphomekitchenmotion_mac_wifi_address"; 
    public const string EsphomekitchenmotionIpAddress = "sensor.esphomekitchenmotion_ip_address"; 
    public const string EsphomekitchenmotionEsphomeVersion = "sensor.esphomekitchenmotion_esphome_version"; 
    public const string EsphomekitchenmotionUptimeHumanReadable = "sensor.esphomekitchenmotion_uptime_human_readable"; 
    public const string EsphomekitchenmotionLd2450Firmware = "sensor.esphomekitchenmotion_ld2450_firmware"; 
    public const string EsphomekitchenmotionLd2450BtMac = "sensor.esphomekitchenmotion_ld2450_bt_mac"; 
    public const string Node2LastSeen = "sensor.node_2_last_seen"; 
    public const string Node3LastSeen = "sensor.node_3_last_seen"; 
    public const string Node4LastSeen = "sensor.node_4_last_seen"; 
    public const string Node5LastSeen = "sensor.node_5_last_seen"; 
    public const string Node6LastSeen = "sensor.node_6_last_seen"; 
    public const string Node7LastSeen = "sensor.node_7_last_seen"; 
    public const string Node8LastSeen = "sensor.node_8_last_seen"; 
    public const string Node10LastSeen = "sensor.node_10_last_seen"; 
    public const string Node12LastSeen = "sensor.node_12_last_seen"; 
    public const string Node14LastSeen = "sensor.node_14_last_seen"; 
    public const string Node15LastSeen = "sensor.node_15_last_seen"; 
    public const string Node16LastSeen = "sensor.node_16_last_seen"; 
    public const string Node17LastSeen = "sensor.node_17_last_seen"; 
    public const string Node18LastSeen = "sensor.node_18_last_seen"; 
    public const string KazulPowerStripLastSeen = "sensor.kazul_power_strip_last_seen"; 
    public const string Node21LastSeen = "sensor.node_21_last_seen"; 
    public const string Node22LastSeen = "sensor.node_22_last_seen"; 
    public const string Node23LastSeen = "sensor.node_23_last_seen"; 
    public const string Node24LastSeen = "sensor.node_24_last_seen"; 
    public const string Node30LastSeen = "sensor.node_30_last_seen"; 
    public const string Node31LastSeen = "sensor.node_31_last_seen"; 
    public const string Node32LastSeen = "sensor.node_32_last_seen"; 
    public const string Node33LastSeen = "sensor.node_33_last_seen"; 
    public const string Node34LastSeen = "sensor.node_34_last_seen"; 
    public const string Node35LastSeen = "sensor.node_35_last_seen"; 
    public const string Node38LastSeen = "sensor.node_38_last_seen"; 
    public const string Node39LastSeen = "sensor.node_39_last_seen"; 
    public const string OutsideDualPlugLastSeen = "sensor.outside_dual_plug_last_seen"; 
    public const string PressureChange4Hr = "sensor.pressure_change_4_hr"; 
    public const string LivingRoomButtonsBatteryLevel = "sensor.living_room_buttons_battery_level"; 
    public const string LoungeButtonsBatteryLevel = "sensor.lounge_buttons_battery_level"; 
    public const string KazulPowerStripElectricConsumptionW = "sensor.kazul_power_strip_electric_consumption_w"; 
    public const string KazulPowerStripElectricConsumptionKwh = "sensor.kazul_power_strip_electric_consumption_kwh"; 
    public const string KazulPowerStripElectricConsumptionV = "sensor.kazul_power_strip_electric_consumption_v"; 
    public const string KazulPowerStripElectricConsumptionA = "sensor.kazul_power_strip_electric_consumption_a"; 
    public const string KazulPowerStripElectricConsumptionKwh1 = "sensor.kazul_power_strip_electric_consumption_kwh_1"; 
    public const string KazulPowerStripElectricConsumptionW1 = "sensor.kazul_power_strip_electric_consumption_w_1"; 
    public const string KazulPowerStripElectricConsumptionV1 = "sensor.kazul_power_strip_electric_consumption_v_1"; 
    public const string KazulPowerStripElectricConsumptionA1 = "sensor.kazul_power_strip_electric_consumption_a_1"; 
    public const string KazulPowerStripElectricConsumptionKwh2 = "sensor.kazul_power_strip_electric_consumption_kwh_2"; 
    public const string KazulPowerStripElectricConsumptionW2 = "sensor.kazul_power_strip_electric_consumption_w_2"; 
    public const string KazulPowerStripElectricConsumptionV2 = "sensor.kazul_power_strip_electric_consumption_v_2"; 
    public const string KazulPowerStripElectricConsumptionA2 = "sensor.kazul_power_strip_electric_consumption_a_2"; 
    public const string KazulPowerStripElectricConsumptionKwh3 = "sensor.kazul_power_strip_electric_consumption_kwh_3"; 
    public const string KazulPowerStripElectricConsumptionW3 = "sensor.kazul_power_strip_electric_consumption_w_3"; 
    public const string KazulPowerStripElectricConsumptionV3 = "sensor.kazul_power_strip_electric_consumption_v_3"; 
    public const string KazulPowerStripElectricConsumptionA3 = "sensor.kazul_power_strip_electric_consumption_a_3"; 
    public const string KazulPowerStripElectricConsumptionKwh4 = "sensor.kazul_power_strip_electric_consumption_kwh_4"; 
    public const string KazulPowerStripElectricConsumptionW4 = "sensor.kazul_power_strip_electric_consumption_w_4"; 
    public const string KazulPowerStripElectricConsumptionV4 = "sensor.kazul_power_strip_electric_consumption_v_4"; 
    public const string KazulPowerStripElectricConsumptionA4 = "sensor.kazul_power_strip_electric_consumption_a_4"; 
    public const string KazulPowerStripElectricConsumptionKwh5 = "sensor.kazul_power_strip_electric_consumption_kwh_5"; 
    public const string KazulPowerStripElectricConsumptionW5 = "sensor.kazul_power_strip_electric_consumption_w_5"; 
    public const string KazulPowerStripElectricConsumptionV5 = "sensor.kazul_power_strip_electric_consumption_v_5"; 
    public const string KazulPowerStripElectricConsumptionA5 = "sensor.kazul_power_strip_electric_consumption_a_5"; 
    public const string BasementMotionBatteryLevel = "sensor.basement_motion_battery_level"; 
    public const string KazulTempHumidityAirTemperature = "sensor.kazul_temp_humidity_air_temperature"; 
    public const string KazulTempHumidityHumidity = "sensor.kazul_temp_humidity_humidity"; 
    public const string KazulTempHumidityBatteryLevel = "sensor.kazul_temp_humidity_battery_level"; 
    public const string Garage1TiltBatteryLevel = "sensor.garage_1_tilt_battery_level"; 
    public const string Garage2TiltBatteryLevel = "sensor.garage_2_tilt_battery_level"; 
    public const string _4In1SensorAirTemperature = "sensor.4_in_1_sensor_air_temperature"; 
    public const string _4In1SensorIlluminance = "sensor.4_in_1_sensor_illuminance"; 
    public const string _4In1SensorHumidity = "sensor.4_in_1_sensor_humidity"; 
    public const string _4In1SensorBatteryLevel = "sensor.4_in_1_sensor_battery_level"; 
    public const string PlantPlug1ElectricConsumptionKwh = "sensor.plant_plug_1_electric_consumption_kwh"; 
    public const string PlantPlug1ElectricConsumptionW = "sensor.plant_plug_1_electric_consumption_w"; 
    public const string PlantPlug1ElectricConsumptionV = "sensor.plant_plug_1_electric_consumption_v"; 
    public const string PlantPlug1ElectricConsumptionA = "sensor.plant_plug_1_electric_consumption_a"; 
    public const string PressureChange8Hr = "sensor.pressure_change_8_hr"; 
    public const string PressureChange12Hr = "sensor.pressure_change_12_hr"; 
    public const string PressureChange24Hr = "sensor.pressure_change_24_hr"; 
    public const string Dv102683GLaundryMachineState = "sensor.dv102683g_laundry_machine_state"; 
    public const string Dv102683GLaundryCycle = "sensor.dv102683g_laundry_cycle"; 
    public const string Dv102683GLaundrySubCycle = "sensor.dv102683g_laundry_sub_cycle"; 
    public const string Dv102683GLaundryTimeRemaining = "sensor.dv102683g_laundry_time_remaining"; 
    public const string Dv102683GLaundryDelayTimeRemaining = "sensor.dv102683g_laundry_delay_time_remaining"; 
    public const string Dv102683GLaundryDryerDrynessnewLevel = "sensor.dv102683g_laundry_dryer_drynessnew_level"; 
    public const string Dv102683GLaundryDryerTemperaturenewOption = "sensor.dv102683g_laundry_dryer_temperaturenew_option"; 
    public const string Dv102683GLaundryDryerTumblenewStatus = "sensor.dv102683g_laundry_dryer_tumblenew_status"; 
    public const string Dv102683GLaundryDryerSheetUsageConfiguration = "sensor.dv102683g_laundry_dryer_sheet_usage_configuration"; 
    public const string Dv102683GLaundryDryerSheetInventory = "sensor.dv102683g_laundry_dryer_sheet_inventory"; 
    public const string Dv102683GLaundryDryerEcodryStatus = "sensor.dv102683g_laundry_dryer_ecodry_status"; 
    public const string Av339078NLaundryMachineState = "sensor.av339078n_laundry_machine_state"; 
    public const string Av339078NLaundryCycle = "sensor.av339078n_laundry_cycle"; 
    public const string Av339078NLaundrySubCycle = "sensor.av339078n_laundry_sub_cycle"; 
    public const string Av339078NLaundryTimeRemaining = "sensor.av339078n_laundry_time_remaining"; 
    public const string Av339078NLaundryDelayTimeRemaining = "sensor.av339078n_laundry_delay_time_remaining"; 
    public const string Av339078NLaundryWasherSoilLevel = "sensor.av339078n_laundry_washer_soil_level"; 
    public const string Av339078NLaundryWasherWashtempLevel = "sensor.av339078n_laundry_washer_washtemp_level"; 
    public const string Av339078NLaundryWasherSpintimeLevel = "sensor.av339078n_laundry_washer_spintime_level"; 
    public const string Av339078NLaundryWasherRinseOption = "sensor.av339078n_laundry_washer_rinse_option"; 
    public const string Av339078NLaundryWasherSmartDispenseLoadsLeft = "sensor.av339078n_laundry_washer_smart_dispense_loads_left"; 
    public const string Av339078NLaundryWasherSmartDispenseTankStatus = "sensor.av339078n_laundry_washer_smart_dispense_tank_status"; 
    public const string Rt149699FridgeModelInfo = "sensor.rt149699_fridge_model_info"; 
    public const string Rt149699DoorStatus = "sensor.rt149699_door_status"; 
    public const string Rt149699IceMakerBucketStatus = "sensor.rt149699_ice_maker_bucket_status"; 
    public const string Rt149699CurrentTemperatureFridge = "sensor.rt149699_current_temperature_fridge"; 
    public const string Rt149699WaterFilterStatus = "sensor.rt149699_water_filter_status"; 
    public const string Rt149699IceMakerBucketStatusStateFullFridge = "sensor.rt149699_ice_maker_bucket_status_state_full_fridge"; 
    public const string Rt149699CurrentTemperatureFreezer = "sensor.rt149699_current_temperature_freezer"; 
    public const string DoorbellRepeater579ABatterySensor = "sensor.doorbell_repeater_579a_battery_sensor"; 
    public const string BasementStairMotionAq2Battery2 = "sensor.basement_stair_motion_aq2_battery_2"; 
    public const string BasementStairMotionAq2Illuminance2 = "sensor.basement_stair_motion_aq2_illuminance_2"; 
    public const string BasementStairMotionAq2DeviceTemperature2 = "sensor.basement_stair_motion_aq2_device_temperature_2"; 
    public const string BackDoorContactBattery = "sensor.back_door_contact_battery"; 
    public const string InsideGarageDoorContactBattery = "sensor.inside_garage_door_contact_battery"; 
    public const string LumiLumiSensorMotionAq2Battery3 = "sensor.lumi_lumi_sensor_motion_aq2_battery_3"; 
    public const string LumiLumiSensorMotionAq2Illuminance3 = "sensor.lumi_lumi_sensor_motion_aq2_illuminance_3"; 
    public const string LumiLumiSensorMotionAq2DeviceTemperature3 = "sensor.lumi_lumi_sensor_motion_aq2_device_temperature_3"; 
    public const string FrontDoorContactBattery = "sensor.front_door_contact_battery"; 
    public const string OfficeDoorBattery = "sensor.office_door_battery"; 
    public const string Garage2ContactBattery = "sensor.garage_2_contact_battery"; 
    public const string Garage1ContactBattery = "sensor.garage_1_contact_battery"; 
    public const string MonkeyInstantaneousDemand = "sensor.monkey_instantaneous_demand"; 
    public const string MonkeySummationDelivered = "sensor.monkey_summation_delivered"; 
    public const string OfficeMotionBattery = "sensor.office_motion_battery"; 
    public const string OfficeMotionIlluminance = "sensor.office_motion_illuminance"; 
    public const string OfficeMotionDeviceTemperature = "sensor.office_motion_device_temperature"; 
    public const string BackHallCoatClosetContactBattery = "sensor.back_hall_coat_closet_contact_battery"; 
    public const string MbrMotion1Battery = "sensor.mbr_motion_1_battery"; 
    public const string MbrMotion2Battery = "sensor.mbr_motion_2_battery"; 
    public const string BasementMotion2Battery = "sensor.basement_motion_2_battery"; 
    public const string Weatheralerts1 = "sensor.weatheralerts_1"; 
    public const string SpotifyLeonardSperrySongTempo = "sensor.spotify_leonard_sperry_song_tempo"; 
    public const string SpotifyLeonardSperrySongMode = "sensor.spotify_leonard_sperry_song_mode"; 
    public const string PortainerEndpointsLocal = "sensor.portainer_endpoints_local"; 
    public const string PortainerLocalHomeAutomationsProd = "sensor.portainer_local_home_automations_prod"; 
    public const string PortainerLocalOtelcollector = "sensor.portainer_local_otelcollector"; 
    public const string PortainerLocalRedis = "sensor.portainer_local_redis"; 
    public const string PortainerLocalPortainer = "sensor.portainer_local_portainer"; 
    public const string PortainerLocalTempo = "sensor.portainer_local_tempo"; 
    public const string PortainerLocalPromtail = "sensor.portainer_local_promtail"; 
    public const string PortainerLocalLoki = "sensor.portainer_local_loki"; 
    public const string PortainerLocalPrometheus = "sensor.portainer_local_prometheus"; 
    public const string PortainerLocalKafkaUi = "sensor.portainer_local_kafka_ui"; 
    public const string PortainerLocalKafka = "sensor.portainer_local_kafka"; 
    public const string PortainerLocalGlances = "sensor.portainer_local_glances"; 
    public const string PortainerLocalDashy = "sensor.portainer_local_dashy"; 
    public const string PortainerLocalTraefik = "sensor.portainer_local_traefik"; 
    public const string PortainerLocalSnmpExporter = "sensor.portainer_local_snmp_exporter"; 
    public const string BackupState = "sensor.backup_state"; 
    public const string PortainerLocalHomeAutomationsProd2 = "sensor.portainer_local_home_automations_prod_2"; 
    public const string PortainerLocalHomeAutomationsProd3 = "sensor.portainer_local_home_automations_prod_3"; 
    public const string PortainerLocalHomeAutomationsProd4 = "sensor.portainer_local_home_automations_prod_4"; 
    public const string PortainerLocalHomeAutomationsProd5 = "sensor.portainer_local_home_automations_prod_5";
}

public class Stt
{  
    public const string HomeAssistantCloud = "stt.home_assistant_cloud"; 
    public const string FasterWhisper = "stt.faster_whisper";
}

public class Sun
{  
    public const string _Sun = "sun.sun";
}

public class Switch
{  
    public const string MbrFloorLights = "switch.mbr_floor_lights"; 
    public const string MbrDadSideSwitch = "switch.mbr_dad_side_switch"; 
    public const string FrontRoomComputerLamp = "switch.front_room_computer_lamp"; 
    public const string UnifiNetworkBlockSamsungFromInternet = "switch.unifi_network_block_samsung_from_internet"; 
    public const string EsphomeLivingRoomRestart = "switch.esphome_living_room_restart"; 
    public const string EsphomeLivingRoomUseSafeMode = "switch.esphome_living_room_use_safe_mode"; 
    public const string EsphomeLivingRoomBluetooth = "switch.esphome_living_room_bluetooth"; 
    public const string EsphomeLivingRoomMultiTargetTracking = "switch.esphome_living_room_multi_target_tracking"; 
    public const string EsphomekitchenmotionRestart = "switch.esphomekitchenmotion_restart"; 
    public const string EsphomekitchenmotionUseSafeMode = "switch.esphomekitchenmotion_use_safe_mode"; 
    public const string EsphomekitchenmotionBluetooth = "switch.esphomekitchenmotion_bluetooth"; 
    public const string EsphomekitchenmotionMultiTargetTracking = "switch.esphomekitchenmotion_multi_target_tracking"; 
    public const string BackHallLight = "switch.back_hall_light"; 
    public const string DiningRoomLights = "switch.dining_room_lights"; 
    public const string DiningRoomLights0X50NodeIdentifyBinary = "switch.dining_room_lights_0x50_node_identify_binary"; 
    public const string DiningRoomLights0X43Button1IndicationBinary = "switch.dining_room_lights_0x43_button_1_indication_binary"; 
    public const string DiningRoomLights0X44Button2IndicationBinary = "switch.dining_room_lights_0x44_button_2_indication_binary"; 
    public const string DiningRoomLights0X45Button3IndicationBinary = "switch.dining_room_lights_0x45_button_3_indication_binary"; 
    public const string DiningRoomLights0X46Button4IndicationBinary = "switch.dining_room_lights_0x46_button_4_indication_binary"; 
    public const string DiningRoomLights0X47Button5IndicationBinary = "switch.dining_room_lights_0x47_button_5_indication_binary"; 
    public const string PeacockLamp = "switch.peacock_lamp"; 
    public const string BackPorchLight = "switch.back_porch_light"; 
    public const string BackFlood = "switch.back_flood"; 
    public const string KazulPowerStrip = "switch.kazul_power_strip"; 
    public const string KazulPowerStrip1 = "switch.kazul_power_strip_1"; 
    public const string KazulPowerStrip2 = "switch.kazul_power_strip_2"; 
    public const string KazulPowerStrip3 = "switch.kazul_power_strip_3"; 
    public const string KazulPowerStrip4 = "switch.kazul_power_strip_4"; 
    public const string KazulPowerStrip5 = "switch.kazul_power_strip_5"; 
    public const string KazulPowerStrip6 = "switch.kazul_power_strip_6"; 
    public const string KazulPowerStrip7 = "switch.kazul_power_strip_7"; 
    public const string BasementStairLight = "switch.basement_stair_light"; 
    public const string BasementStairLight0X50NodeIdentifyBinary = "switch.basement_stair_light_0x50_node_identify_binary"; 
    public const string BasementStairLight0X43Button1IndicationBinary = "switch.basement_stair_light_0x43_button_1_indication_binary"; 
    public const string BasementStairLight0X44Button2IndicationBinary = "switch.basement_stair_light_0x44_button_2_indication_binary"; 
    public const string BasementStairLight0X45Button3IndicationBinary = "switch.basement_stair_light_0x45_button_3_indication_binary"; 
    public const string BasementStairLight0X46Button4IndicationBinary = "switch.basement_stair_light_0x46_button_4_indication_binary"; 
    public const string BasementStairLight0X47Button5IndicationBinary = "switch.basement_stair_light_0x47_button_5_indication_binary"; 
    public const string GarageDoorOpener = "switch.garage_door_opener"; 
    public const string PlantPlug1 = "switch.plant_plug_1"; 
    public const string MbrDresserPlug = "switch.mbr_dresser_plug"; 
    public const string OutsideDualPlug = "switch.outside_dual_plug"; 
    public const string OutsideDualPlug2 = "switch.outside_dual_plug_2"; 
    public const string Rt149699SabbathMode = "switch.rt149699_sabbath_mode"; 
    public const string Rt149699FridgeIceBoost = "switch.rt149699_fridge_ice_boost"; 
    public const string Rt149699IceMakerControl = "switch.rt149699_ice_maker_control"; 
    public const string Rt149699ProximityLight = "switch.rt149699_proximity_light"; 
    public const string Rt149699TurboFreezeStatus = "switch.rt149699_turbo_freeze_status"; 
    public const string DoorbellRepeater579AMute = "switch.doorbell_repeater_579a_mute"; 
    public const string DoorbellRepeater579AMute2 = "switch.doorbell_repeater_579a_mute_2"; 
    public const string DoorbellRepeater579AMute3 = "switch.doorbell_repeater_579a_mute_3"; 
    public const string DiningRoomSpeakerCrossfade = "switch.dining_room_speaker_crossfade"; 
    public const string DiningRoomSpeakerLoudness = "switch.dining_room_speaker_loudness"; 
    public const string AsherRoomSpeakerCrossfade = "switch.asher_room_speaker_crossfade"; 
    public const string AsherRoomSpeakerLoudness = "switch.asher_room_speaker_loudness"; 
    public const string MainBedroomSpeakerCrossfade = "switch.main_bedroom_speaker_crossfade"; 
    public const string MainBedroomSpeakerLoudness = "switch.main_bedroom_speaker_loudness"; 
    public const string OfficeFanSwitch = "switch.office_fan_switch";
}

public class Timer
{  
    public const string Testtimer = "timer.testtimer";
}

public class Todo
{  
    public const string ShoppingList = "todo.shopping_list"; 
    public const string Leo = "todo.leo";
}

public class Tts
{  
    public const string HomeAssistantCloud = "tts.home_assistant_cloud"; 
    public const string Piper = "tts.piper";
}

public class Update
{  
    public const string HomeAssistantSupervisorUpdate = "update.home_assistant_supervisor_update"; 
    public const string HomeAssistantCoreUpdate = "update.home_assistant_core_update"; 
    public const string AdvancedSshWebTerminalUpdate = "update.advanced_ssh_web_terminal_update"; 
    public const string HomeAssistantGoogleDriveBackupUpdate = "update.home_assistant_google_drive_backup_update"; 
    public const string FileEditorUpdate = "update.file_editor_update"; 
    public const string MatterServerUpdate = "update.matter_server_update"; 
    public const string StudioCodeServerUpdate = "update.studio_code_server_update"; 
    public const string PiperUpdate = "update.piper_update"; 
    public const string WhisperUpdate = "update.whisper_update"; 
    public const string OpenwakewordUpdate = "update.openwakeword_update"; 
    public const string SambaShareUpdate = "update.samba_share_update"; 
    public const string EsphomeUpdate = "update.esphome_update"; 
    public const string MariadbUpdate = "update.mariadb_update"; 
    public const string InfluxdbUpdate = "update.influxdb_update"; 
    public const string GrafanaUpdate = "update.grafana_update"; 
    public const string MosquittoBrokerUpdate = "update.mosquitto_broker_update"; 
    public const string ZWaveJsUiUpdate = "update.z_wave_js_ui_update"; 
    public const string OpenthreadBorderRouterUpdate = "update.openthread_border_router_update"; 
    public const string HomeAssistantOperatingSystemUpdate = "update.home_assistant_operating_system_update"; 
    public const string MbrDadSideSwitch = "update.mbr_dad_side_switch"; 
    public const string FrontRoomComputerLamp = "update.front_room_computer_lamp"; 
    public const string Picard = "update.picard"; 
    public const string Heimdall = "update.heimdall"; 
    public const string MultilineEntityCardUpdate = "update.multiline_entity_card_update"; 
    public const string HacsUpdate = "update.hacs_update"; 
    public const string SaverUpdate = "update.saver_update"; 
    public const string ExtendedOpenaiConversationUpdate = "update.extended_openai_conversation_update"; 
    public const string WatchmanUpdate = "update.watchman_update"; 
    public const string NwsAlertsUpdate = "update.nws_alerts_update"; 
    public const string ToggleControlButtonRowUpdate = "update.toggle_control_button_row_update"; 
    public const string GeHomeSmarthqUpdate = "update.ge_home_smarthq_update"; 
    public const string PortainerUpdate = "update.portainer_update"; 
    public const string EsphomeLivingRoomFirmware = "update.esphome_living_room_firmware"; 
    public const string OfficeDisplayLightsFirmware = "update.office_display_lights_firmware"; 
    public const string KitchenLightsFirmware = "update.kitchen_lights_firmware"; 
    public const string FrontRoomLightFirmware = "update.front_room_light_firmware"; 
    public const string FrontPorchLightFirmware = "update.front_porch_light_firmware"; 
    public const string EntryLightFirmware = "update.entry_light_firmware"; 
    public const string UpstairsHallFirmware = "update.upstairs_hall_firmware"; 
    public const string CraftRoomLightFirmware = "update.craft_room_light_firmware"; 
    public const string BasementLight3Firmware = "update.basement_light_3_firmware"; 
    public const string BasementLight1Firmware = "update.basement_light_1_firmware"; 
    public const string BasementLight2Firmware = "update.basement_light_2_firmware"; 
    public const string BasementMotionFirmware = "update.basement_motion_firmware"; 
    public const string KazulTempHumidityFirmware = "update.kazul_temp_humidity_firmware"; 
    public const string _4In1SensorFirmware = "update.4_in_1_sensor_firmware"; 
    public const string BackHallLightFirmware = "update.back_hall_light_firmware"; 
    public const string DiningRoomLightsFirmware = "update.dining_room_lights_firmware"; 
    public const string PeacockLampFirmware = "update.peacock_lamp_firmware"; 
    public const string BackPorchLightFirmware = "update.back_porch_light_firmware"; 
    public const string BackFloodFirmware = "update.back_flood_firmware"; 
    public const string KazulPowerStripFirmware = "update.kazul_power_strip_firmware"; 
    public const string BasementStairLightFirmware = "update.basement_stair_light_firmware"; 
    public const string GarageDoorOpenerFirmware = "update.garage_door_opener_firmware"; 
    public const string PlantPlug1Firmware = "update.plant_plug_1_firmware"; 
    public const string MbrDresserPlugFirmware = "update.mbr_dresser_plug_firmware"; 
    public const string OutsideDualPlugFirmware = "update.outside_dual_plug_firmware"; 
    public const string BasementStairMotionFirmware = "update.basement_stair_motion_firmware"; 
    public const string BackDoorContactFirmware = "update.back_door_contact_firmware"; 
    public const string InsideGarageDoorContactFirmware = "update.inside_garage_door_contact_firmware"; 
    public const string FrontPorchMotionFirmware = "update.front_porch_motion_firmware"; 
    public const string FrontDoorContactFirmware = "update.front_door_contact_firmware"; 
    public const string OfficeDoorFirmware = "update.office_door_firmware"; 
    public const string OfficeLedFirmware = "update.office_led_firmware"; 
    public const string Garage2ContactFirmware = "update.garage_2_contact_firmware"; 
    public const string Garage1ContactFirmware = "update.garage_1_contact_firmware"; 
    public const string MonkeyFirmware = "update.monkey_firmware"; 
    public const string OfficeMotionFirmware = "update.office_motion_firmware"; 
    public const string BackHallCoatClosetContactFirmware = "update.back_hall_coat_closet_contact_firmware"; 
    public const string MbrMotion1Firmware = "update.mbr_motion_1_firmware"; 
    public const string MbrMotion2Firmware = "update.mbr_motion_2_firmware"; 
    public const string BasementMotion2Firmware = "update.basement_motion_2_firmware";
}

public class Wake_Word
{  
    public const string Openwakeword = "wake_word.openwakeword";
}

public class Water_Heater
{  
    public const string Rt149699Fridge = "water_heater.rt149699_fridge"; 
    public const string Rt149699Freezer = "water_heater.rt149699_freezer";
}

public class Weather
{  
    public const string ForecastHome = "weather.forecast_home"; 
    public const string Kthv = "weather.kthv";
}

public class Zone
{  
    public const string ManchesterGiant = "zone.manchester_giant"; 
    public const string Work = "zone.work"; 
    public const string Home = "zone.home";
}