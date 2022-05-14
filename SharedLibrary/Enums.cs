namespace SharedLibrary
{
    public enum ServerToClientId : ushort
    {
        Sync = 1,
        ActiveScene,
        PlayerSpawned,
        PlayerMovement,
        PlayerHealthChanged,
        PlayerActiveWeaponUpdated,
        PlayerAmmoChanged,
        PlayerDied,
        PlayerRespawned,
        ProjectileSpawned,
        ProjectileMovement,
        ProjectileCollided,
        ProjectileHitMarker,
    }

    public enum ClientToServerId : ushort
    {
        Name = 1,
        Input,
        SwitchActiveWeapon,
        PrimaryUse,
        Reload,
    }
}