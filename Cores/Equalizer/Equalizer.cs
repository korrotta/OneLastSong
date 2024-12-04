using NAudio.Wave;
using NAudio.Dsp;
using System;
using System.Linq;
using OneLastSong.Cores.Equalizer;

public class Equalizer : ISampleProvider
{
    private readonly ISampleProvider source;
    private readonly EqualizerBand[] bands;
    private readonly BiQuadFilter[] filters;

    public Equalizer(ISampleProvider source, EqualizerBand[] bands)
    {
        this.source = source;
        this.bands = bands;
        this.filters = bands.Select(b => BiQuadFilter.PeakingEQ(source.WaveFormat.SampleRate, b.Frequency, b.Bandwidth, b.Gain)).ToArray();
    }

    public WaveFormat WaveFormat => source.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = source.Read(buffer, offset, count);
        for (int i = 0; i < samplesRead; i++)
        {
            float sample = buffer[offset + i];
            foreach (var filter in filters)
            {
                sample = filter.Transform(sample);
            }
            buffer[offset + i] = sample;
        }
        return samplesRead;
    }

    public void Update(EqualizerBand[] newBands)
    {
        for (int i = 0; i < bands.Length; i++)
        {
            bands[i].Gain = newBands[i].Gain;
            filters[i] = BiQuadFilter.PeakingEQ(source.WaveFormat.SampleRate, bands[i].Frequency, bands[i].Bandwidth, bands[i].Gain);
        }
    }
}
