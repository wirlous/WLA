using UnityEngine;

namespace WLA
{
    [System.Serializable]
    public class SwordData
    {
        public string version;
        public SwordDataStructure[] swords;
    }

    [System.Serializable]
    public class SwordDataStructure
    {
        public string name;
        public int code;
        public int damage;
        public ColliderDefinition upCollider;
        public ColliderDefinition downCollider;
        public ColliderDefinition rightCollider;
        public ColliderDefinition leftCollider;

        public SwordDataStructure(string name, int code, int damage, ColliderDefinition up, ColliderDefinition down, ColliderDefinition right, ColliderDefinition left)
        {
            this.name = name;
            this.code = code;
            this.damage = damage;
            this.upCollider = up;
            this.downCollider = down;
            this.rightCollider = right;
            this.leftCollider = left;
        }

        public SwordDataStructure()
        {
            this.name = "";
            this.code = -1;
            this.damage = 0;
            this.upCollider = null;
            this.downCollider = null;
            this.rightCollider = null;
            this.leftCollider = null;
        }

    }

    [System.Serializable]
    public class ColliderDefinition
    {
        public float offsetX;
        public float offsetY;
        public float width;
        public float height;

        public ColliderDefinition(float offsetX, float offsetY, float width, float height)
        {
            this.offsetX    = offsetX;
            this.offsetY    = offsetY;
            this.width      = width;
            this.height     = height;
        }

        public ColliderDefinition()
        {
            this.offsetX    = 0f;
            this.offsetY    = 0f;
            this.width      = 1f;
            this.height     = 1f;
        }

        public Vector2 GetOffset()
        {
            return new Vector2(offsetX, offsetY);
        }

        public Vector2 GetSize()
        {
            return new Vector2(width, height);
        }

    }
}
