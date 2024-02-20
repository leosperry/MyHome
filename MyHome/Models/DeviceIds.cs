namespace MyHome;

public static class Lights
{
    public const string
        OfficeLights = "light.office_lights",
        OfficeLeds = "light.office_led_light",
        BackHallLight = "switch.back_hall_light",
        KitchenLights = "light.kitchen_lights",
        DiningRoomLights = "switch.dining_room",
        BackPorch = "switch.back_porch_light",
        BackFlood = "switch.back_flood",
        CounchOverhead = "light.couch_overhead",
        Couch1 = "light.wiz_rgbw_tunable_79a59c",
        Couch2 = "light.wiz_rgbw_tunable_79aab4",
        Couch3 = "light.wiz_rgbw_tunable_8d7d54",
        TvBacklight = "light.tv_backlight",
        LivingLamp1 = "light.living_lamp_1",
        LivingLamp2 = "light.living_lamp_2",
        PeacockLamp = "switch.peacock_lamp",
        FrontRoomLight = "light.front_room_light",
        EntryLight = "light.entry_light",
        FrontPorchLight = "light.front_porch",
        LoungeCeiling = "light.lounge_lights",
        UpstairsHall = "light.upstairs_hall",
        MainBedroomLight1 = "light.main_bedroom_light_1",
        MainBedroomLight2 = "light.main_bedroom_light_2",
        CraftRoomLights = "light.craft_room", 
        BasementStair = "switch.basement_stair",
        Basement1 = "light.basement_1",
        Basement2 = "light.basement_2",
        BasementWork = "light.basement_work";
}

public static class Devices
{
    public const string
        Roku = "remote.roku_ultra",
        OfficeFan = "automation.office_fan",
        OfficeDoor = "binary_sensor.office_door_opening";
}

public static class NotificationGroups
{
    public const string
        Critical = "critical_notification_group";
}

public static class Alexa
{
    public const string 
        Office = "Office",
        LivingRoome = "Living Room",
        Kitchen = "Kitchen",
        Asher = "Asher",
        Logan = "Logan",
        Lyra = "Lyra",
        MainBedroom = "Main Bedroom";
}

public static class Helpers
{
    public const string
        BedTime = "input_boolean.bedtime_switch";
}