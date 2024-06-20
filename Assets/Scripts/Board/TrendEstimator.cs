using UnityEngine;

namespace OpenCvSharp.Demo 
{ 
    public class TrendEstimator
    {
        private HoltFilter[] position;
        private HoltFilter[] rotation;
        private float alpha, beta;

        public TrendEstimator(float alpha = 0.2f, float beta = 0.8f)
        {
            this.alpha = alpha;
            this.beta = beta;
            position = new HoltFilter[3];
            rotation = new HoltFilter[3];
            for(var i=0; i<3; i++)
            {
                position[i] = new HoltFilter(alpha, beta);
                rotation[i] = new HoltFilter(alpha, beta);
            }
        }

        public Vector3 UpdatePosition(Vector3 v)
        {
            Vector3 upd;
            upd.x = position[0].Update(v.x);
            upd.y = position[1].Update(v.y);
            upd.z = position[2].Update(v.z);
            return upd;
        }

        public Quaternion UpdateRotation(Quaternion v)
        {
            Vector3 v3 = v.eulerAngles;
            Vector3 upd;
            upd.x = rotation[0].Update(v3.x);
            upd.y = rotation[1].Update(v3.y);
            upd.z = rotation[2].Update(v3.z);
            return Quaternion.Euler(upd);
        }
    }
}
