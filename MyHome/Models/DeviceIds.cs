namespace MyHome;

public static class Lights
{
    public const string
        BackHallLight = "switch.back_hall_light",
        BackPorch = "switch.back_porch_light",
        BackFlood = "switch.back_flood",
        BasementStair = "switch.basement_stair",
        Basement1 = "light.basement_1",
        Basement2 = "light.basement_2",
        BasementWork = "light.basement_work",
        CounchOverhead = "light.couch_overhead",
        Couch1 = "light.wiz_rgbw_tunable_79a59c",
        Couch2 = "light.wiz_rgbw_tunable_79aab4",
        Couch3 = "light.wiz_rgbw_tunable_8d7d54",
        CraftRoomLights = "light.craft_room", 
        DiningRoomLights = "switch.dining_room",
        EntryLight = "light.entry_light",
        FrontRoomLight = "light.front_room_light",
        FrontPorchLight = "light.front_porch",
        KitchenLights = "light.kitchen_lights",
        LivingLamp1 = "light.living_lamp_1",
        LivingLamp2 = "light.living_lamp_2",
        LoungeCeiling = "light.lounge_lights",
        MainBedroomLight1 = "light.main_bedroom_light_1",
        MainBedroomLight2 = "light.main_bedroom_light_2",
        Monkey = "light.monkey_light",
        OfficeLights = "light.office_lights",
        OfficeLeds = "light.office_led_light",
        OfficeLightBars = "light.office_light_bars",
        PeacockLamp = "switch.peacock_lamp",
        TvBacklight = "light.tv_backlight",
        UpstairsHall = "light.upstairs_hall";
}

public static class Devices
{
    public const string
        FrontDoorLock = "lock.aqara_smart_lock_u100",
        Roku = "remote.roku_ultra",
        OfficeFan = "switch.office_fan_switch",
        SolarPower = "sensor.solaredge_current_power";
}

public static class Sensors
{
    public const string
        BasementMotion = "binary_sensor.basement_motion_motion_detection",
        BasementStairMotion = "binary_sensor.lumi_lumi_sensor_motion_aq2_motion_2",
        FrontPorchMotion = "binary_sensor.lumi_lumi_sensor_motion_aq2_motion_3",
        MainBedroom4in1Motion = "binary_sensor.4_in_1_sensor_motion_detection",
        OfficeDoor = "binary_sensor.office_door_opening",
        OfficeMotion = "binary_sensor.lumi_lumi_sensor_motion_aq2_motion",
        OfficeTemp = "sensor.lumi_lumi_sensor_motion_aq2_device_temperature",
        KitchenZone1AllCount = "sensor.esphomekitchenmotion_zone_1_all_target_count",
        KitchenZone2AllCount = "sensor.esphomekitchenmotion_zone_2_all_target_count",
        KitchenPresence = "binary_sensor.esphomekitchenmotion_presence",
        LivingRoomPresence = "binary_sensor.esphome_living_room_presence",
        LivingRoomZone1Count = "sensor.esphome_living_room_zone_1_all_target_count",
        LivingRoomZone2Count = "sensor.esphome_living_room_zone_2_all_target_count",
        LivingRoomZone3AllCount = "sensor.esphome_living_room_zone_3_all_target_count",
        LivingRoomAndKitchenPresenceCount = "sensor.livingroomandkitchenpresencecount";
}

public static class NotificationGroups
{
    public const string
        Critical = "critical_notification_group";
}

public static class Phones
{
    public const string        
        LeonardPhone = "mobile_app_leonard_phone",
        RachelPhone = "mobile_app_rachel_phone";

}

public static class MediaPlayers
{
    public const string
        Roku = "media_player.roku_ultra",
        Asher = "media_player.asher_room_speaker",
        //Kitchen = "media_player.kitchen",
        //LivingRoom = "media_player.living_room",
        Lyra = "media_player.lyra",
        MainBedroom = "media_player.main_bedroom_speaker",
        //Office = "media_player.office",
        DiningRoom = "media_player.dining_room_speaker";
    
    public const float
        DiningRoomActiveVolume = 0.35f,
        DiningRoomInActiveVolume = 0.25f;

}

// public static class Alexa
// {
//     public const string 
//         Office = "Office",
//         LivingRoom = "Living Room",
//         Kitchen = "Kitchen",
//         Asher = "Asher",
//         Logan = "Logan",
//         Lyra = "Lyra",
//         MainBedroom = "Main Bedroom";
// }

public static class Helpers
{
    public const string
        BedTime = "input_boolean.bedtime_switch",
        LivingRoomOverride = "input_boolean.living_room_override",
        MaintenanceMode = "input_boolean.maintenance_mode",
        OfficeOverride = "input_boolean.office_override",
        //PorchMotionEnable = "input_boolean.front_porch_motion_enable",
        RachelPhoneBatteryHelper = "binary_sensor.rachelphonebattlowhelper",
        HouseActiveTimesOfDay = "binary_sensor.house_active_times_of_day";

}