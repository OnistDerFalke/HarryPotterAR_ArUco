using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenCvSharp.Demo
{
    public class HoltFilter
    {
        private float alpha;
        private float beta;
        private float lastSmoothedValue;
        private float lastTrend;

        public HoltFilter(float alpha, float beta)
        {
            this.alpha = alpha;
            this.beta = beta;
            lastSmoothedValue = float.NaN;
            lastTrend = float.NaN;
        }

        public float Update(float newValue)
        {
            if (float.IsNaN(lastSmoothedValue))
            {
                lastSmoothedValue = newValue;
                lastTrend = 0;
                return newValue;
            }

            float smoothedValue = alpha * newValue + (1 - alpha) * (lastSmoothedValue + lastTrend);
            float trend = beta * (smoothedValue - lastSmoothedValue) + (1 - beta) * lastTrend;

            lastSmoothedValue = smoothedValue;
            lastTrend = trend;

            return smoothedValue;
        }
    }
}