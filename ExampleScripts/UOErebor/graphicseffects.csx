public class GraphicsEffect
{
    public static readonly GraphicsEffect Sparkles = new GraphicsEffect(0x375A);
    public static readonly GraphicsEffect Spike = new GraphicsEffect(0x37c4);
    public static readonly GraphicsEffect Explosion = new GraphicsEffect(0x36b0);
    public static readonly GraphicsEffect RoundAttack = new GraphicsEffect(0x2ED0);
    public static readonly GraphicsEffect Smoke = new GraphicsEffect(0x3728);
    public static readonly GraphicsEffect FireSnake = new GraphicsEffect(0x36f4);
    public static readonly GraphicsEffect MeteorSwarm = new GraphicsEffect(0x2EB2);
    public static readonly GraphicsEffect Vortex = new GraphicsEffect(0x3789);
    public static readonly GraphicsEffect FieldOfBlades = new GraphicsEffect(0x37a0);
    public static readonly GraphicsEffect Glow = new GraphicsEffect(0x37b9);
    public static readonly GraphicsEffect Glow2 = new GraphicsEffect(0x37be);
    public static readonly GraphicsEffect VortexFull = new GraphicsEffect(0x37cc);
    public static readonly GraphicsEffect EnergyRay = new GraphicsEffect(0x3818);
    public static readonly GraphicsEffect LavaExplosion = new GraphicsEffect(0x1a75);
    public static readonly GraphicsEffect EnergyTwister = new GraphicsEffect(0x3934);
    public static readonly GraphicsEffect Tornado = new GraphicsEffect(0x39c4);
    public static readonly GraphicsEffect Sleeping = new GraphicsEffect(0x3105);
    public static readonly GraphicsEffect RootsCatch = new GraphicsEffect(0x310d);
    public static readonly GraphicsEffect MushroomExplode = new GraphicsEffect(0x1126);
    public static readonly GraphicsEffect SpinningHead = new GraphicsEffect(0x1f1f);
    public static readonly GraphicsEffect AcidSplash = new GraphicsEffect(0x374a);

    public ModelId EffectId { get; }
    public byte Duration { get; }
    public byte Speed { get; }
    
    public GraphicsEffect(ModelId effectId, byte duration = 10, byte speed = 6)
    {
        EffectId = effectId;
        Duration = duration;
    }
    
    public void PlayOverPlayer()
    {
        UO.Client.PlayGraphicalEffect(Infusion.Packets.Server.EffectDirectionType.StayWithSource,
            UO.Me.PlayerId, EffectId, UO.Me.Location, Speed, Duration, true, true);
    }
}