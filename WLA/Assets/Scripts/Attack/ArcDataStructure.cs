using UnityEngine;

namespace WLA
{
    [System.Serializable]
    public class ArcData
    {
        public string version;
        public ArcDataStructure[] arcs;
    }

    [System.Serializable]
    public class ArcDataStructure
    {
        public string name;
        public int code;
        public int damage;
        public int maxAmmunition;
        public ProyectileDefinition proyectile;

        public ArcDataStructure(string name, int code, int damage, int maxAmmunition, ProyectileDefinition proyectile)
        {
            this.name = name;
            this.code = code;
            this.damage = damage;
            this.maxAmmunition = maxAmmunition;
            this.proyectile = proyectile;
        }

        public ArcDataStructure()
        {
            this.name = "";
            this.code = -1;
            this.damage = 0;
            this.maxAmmunition = 0;
            this.proyectile = null;
        }

    }

    [System.Serializable]
    public class ProyectileDefinition
    {
        public float radius;
        public float height;
        public float speed;
        public float maxDistance;

        public ProyectileDefinition(float radius, float height, float speed, float maxDistance)
        {
            this.radius         = radius;
            this.height         = height;
            this.speed          = speed;
            this.maxDistance    = maxDistance;
        }

        public ProyectileDefinition()
        {
            this.radius         = 0.5f;
            this.height         = 1f;
            this.speed          = 1f;
            this.maxDistance    = 1f;
        }

    }
}
