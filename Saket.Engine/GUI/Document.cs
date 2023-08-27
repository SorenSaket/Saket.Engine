using Saket.ECS;
using Saket.Engine.GUI.Styling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Saket.Engine.GUI
{
    public enum EventType
    {
        Enter,
        Stay,
        Exit,
        Click,
        Hold,
    }


    // Do some kind of union shit here
    // which we had rust style enums or something
    public struct Event
    {
        public EventType type;

        public ulong data;
    }



    public ref struct GUIEntityInfo
    {
        public ECSPointer parent;
        public string id;
        public Style style;

        public GUIEntityInfo(ECSPointer parent = default, string id = null, Style style = default)
        {
            this.parent = parent;
            this.id = id;
            this.style = style;
        }
    }

    public class Document
    {
        public event Action<EventType, Entity> OnGUIEvent;



        // IDS and classes should be common across document instances
        /// <summary>
        /// Hashcode -> index
        /// </summary>
        public static Dictionary<int, int> idshashes = new ();
        /// <summary>
        /// Hashcode -> Hashset<index>
        /// </summary>
        public static Dictionary<int, HashSet<int>> classGroups = new();

        public static int GetID(string id)
        {
            if (id == null)
                return -1;

            int hash = id.GetHashCode();

            if (idshashes.ContainsKey(hash))
                return idshashes[hash];

            return -1;
        }

        protected static int GetAndRegisterID(string id)
        {
            if (id == null)
                return -1;

            int hash = id.GetHashCode();

            if (idshashes.ContainsKey(hash))
                return idshashes[hash];
            else
            {
                int value = idshashes.Count;
                idshashes.Add(hash, value);
                return value;
            }
        }

        protected static int GetAndRegisterClassGroup(Span<string> classes)
        {
            if (classes == null || classes.Length == 0) return -1;

            HashSet<int> hashes = new (classes.Length);
            for (int i = 0; i < classes.Length; i++)
            {
                hashes.Add(classes[i].GetHashCode());
            }

            int hashcode = hashes.Count;
            foreach (int val in hashes)
            {
                hashcode = unchecked(hashcode * 13 + val);
            }

            if(classGroups.ContainsKey(hashcode))
                return hashcode;

            int value = classGroups.Count;
            classGroups.Add(hashcode, hashes);
            return value;
        }


        static Query query = new Query().With<(Widget, GUILayout)>();
        // string are managed because you cannot store variable data sizes in an ECS
        public List<string> strings = new List<string>();

        //public List<SpriteElement> batch;

        public World world;
        public StyleSheet styles;

        public Document(World world)
        {
            this.world = world;
        }

        public Entity CreateGUIEntity(GUIEntityInfo info)
        {
            var entity = world.CreateEntity();

            // Assign ID and classes
            entity.Add(new Widget()
            {
                id = GetAndRegisterID(info.id),
            });
            
            entity.Add(new GUILayout());
            

            entity.Add(new HierarchyEntity(info.parent));
            // TODO handle HierarchyEntity referenes

            // Add style directly since it can be considered a component
            entity.Add(info.style);

            return entity;
        }

        public Entity AddText(Entity entity, string text)
        {
            entity.Add(new Text(strings.Count));
            strings.Add(text);
            return entity;
        }

        /* 
        public void Render(RendererSpriteSimple renderer)
        {
            var widgets = world.Query(query);
            foreach (var entity in widgets)
            {
                var widget = entity.Get<Widget>();
                var layoutElement = entity.Get<GUILayout>();






            }
        }*/
    }
}