using UnityEngine;
using System.Collections;

public enum TweenMode
{
    EaseIn,
    EaseOut,
    EaseInOut
}

public class Tween<TPolicy> where TPolicy : TweenPolicy, new()
{
    TPolicy Policy = new TPolicy();
    float elapsedTime = 0;
    float transitionTime = 1;
    float start = 0;
    float end = 0;
    TweenMode easeMode;


    public void Init(float start, float end, float transitionTime, TweenMode easeMode)
    {
        this.start = start;
        this.end = end;
        this.transitionTime = transitionTime;
        this.easeMode = easeMode;
        elapsedTime = 0;
    }

    public void Update(float deltaTime)
    {
        elapsedTime += deltaTime;
    }

    public float Value
    {
        get
        {
            var t = Mathf.Clamp01(elapsedTime / transitionTime);
            float result = 0;
            switch(easeMode)
            {
                case TweenMode.EaseIn: result = Policy.In(t); break;
                case TweenMode.EaseOut: result = Policy.Out(t); break;
                case TweenMode.EaseInOut: result = Policy.InOut(t); break;
            }
            return start + (end - start) * result;
        }
    }
}


public interface TweenPolicy
{
    float In(float t);
    float Out(float t);
    float InOut(float t);
}

public class LinearTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        return t;
    }

    public float Out(float t)
    {
        return t;
    }

    public float InOut(float t)
    {
        return t;
    }
};

public class QuadraticTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        return t * t;
    }

    public float Out(float t)
    {
        return t * (2f - t);
    }

    public float InOut(float t)
    {
        if ((t *= 2f) < 1f) return 0.5f * t * t;
        return -0.5f * ((t -= 1f) * (t - 2f) - 1f);
    }
};

public class CubicTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        return t * t * t;
    }

    public float Out(float t)
    {
        return 1f + ((t -= 1f) * t * t);
    }

    public float InOut(float t)
    {
        if ((t *= 2f) < 1f) return 0.5f * t * t * t;
        return 0.5f * ((t -= 2f) * t * t + 2f);
    }
};

public class QuarticTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        return t * t * t * t;
    }

    public float Out(float t)
    {
        return 1f - ((t -= 1f) * t * t * t);
    }

    public float InOut(float t)
    {
        if ((t *= 2f) < 1f) return 0.5f * t * t * t * t;
        return -0.5f * ((t -= 2f) * t * t * t - 2f);
    }
};

public class QuinticTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        return t * t * t * t * t;
    }

    public float Out(float t)
    {
        return 1f + ((t -= 1f) * t * t * t * t);
    }

    public float InOut(float t)
    {
        if ((t *= 2f) < 1f) return 0.5f * t * t * t * t * t;
        return 0.5f * ((t -= 2f) * t * t * t * t + 2f);
    }
};

public class SinusoidalTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        return 1f - Mathf.Cos(t * Mathf.PI / 2f);
    }

    public float Out(float t)
    {
        return Mathf.Sin(t * Mathf.PI / 2f);
    }

    public float InOut(float t)
    {
        return 0.5f * (1f - Mathf.Cos(Mathf.PI * t));
    }
};

public class ExponentialTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        return t == 0f ? 0f : Mathf.Pow(1024f, t - 1f);
    }

    public float Out(float t)
    {
        return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
    }

    public float InOut(float t)
    {
        if (t == 0f) return 0f;
        if (t == 1f) return 1f;
        if ((t *= 2f) < 1f) return 0.5f * Mathf.Pow(1024f, t - 1f);
        return 0.5f * (-Mathf.Pow(2f, -10f * (t - 1f)) + 2f);
    }
};

public class CircularTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        return 1f - Mathf.Sqrt(1f - t * t);
    }

    public float Out(float t)
    {
        return Mathf.Sqrt(1f - ((t -= 1f) * t));
    }

    public float InOut(float t)
    {
        if ((t *= 2f) < 1f) return -0.5f * (Mathf.Sqrt(1f - t * t) - 1);
        return 0.5f * (Mathf.Sqrt(1f - (t -= 2f) * t) + 1f);
    }
};

public class ElasticTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        if (t == 0) return 0;
        if (t == 1) return 1;
        return -Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t - 0.1f) * (2f * Mathf.PI) / 0.4f);
    }

    public float Out(float t)
    {
        if (t == 0) return 0;
        if (t == 1) return 1;
        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - 0.1f) * (2f * Mathf.PI) / 0.4f) + 1f;
    }

    public float InOut(float t)
    {
        if ((t *= 2f) < 1f) return -0.5f * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t - 0.1f) * (2f * Mathf.PI) / 0.4f);
        return Mathf.Pow(2f, -10f * (t -= 1f)) * Mathf.Sin((t - 0.1f) * (2f * Mathf.PI) / 0.4f) * 0.5f + 1f;
    }
};

public class BackTweenPolicy : TweenPolicy
{
    static float s = 1.70158f;
    static float s2 = 2.5949095f;

    public float In(float t)
    {
        return t * t * ((s + 1f) * t - s);
    }

    public float Out(float t)
    {
        return (t -= 1f) * t * ((s + 1f) * t + s) + 1f;
    }

    public float InOut(float t)
    {
        if ((t *= 2f) < 1f) return 0.5f * (t * t * ((s2 + 1f) * t - s2));
        return 0.5f * ((t -= 2f) * t * ((s2 + 1f) * t + s2) + 2f);
    }
};

public class BounceTweenPolicy : TweenPolicy
{
    public float In(float t)
    {
        return 1f - Out(1f - t);
    }

    public float Out(float t)
    {
        if (t < (1f / 2.75f))
        {
            return 7.5625f * t * t;
        }
        else if (t < (2f / 2.75f))
        {
            return 7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f;
        }
        else if (t < (2.5f / 2.75f))
        {
            return 7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f;
        }
        else
        {
            return 7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f;
        }
    }

    public float InOut(float t)
    {
        if (t < 0.5f) return In(t * 2f) * 0.5f;
        return Out(t * 2f - 1f) * 0.5f + 0.5f;
    }
};