using Microsoft.Xna.Framework.Audio;

namespace Shared;

public static class Audio
{
    private const float PitchVariance = 0.33f;
    
    private static readonly Dictionary<Sound, SoundEffect> Sounds = new();
    private static readonly Random Random = new();

    public static void LoadSound(Sound target, string path)
    {
        Sounds.Add(target, SoundEffect.FromFile(path));
    }

    public static void PlaySound(Sound sound, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
        Sounds[sound].Play(volume, pitch, pan);
    }

    public static void PlaySoundWithPitch(Sound sound, float volume = 1f, float pan = 0f)
    {
        PlaySound(sound, volume, PitchVariance * Random.NextSingle(), pan);
    }
}