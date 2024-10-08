using Saket.ECS;
using Saket.Engine.Graphics.D2.Renderers;
using Saket.Engine.GUI.Styling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using WebGpuSharp;


namespace Saket.Engine.GUI;

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


        var he = new HierarchyEntity(info.parent);

        // handle HierarchyEntity referenes
        if (world.TryGetEntity(info.parent, out var parentEntity))
        {
            var phe = parentEntity.Get<HierarchyEntity>();

            // If the parent has no children yet
            if (phe.last_child == default)
            {
                // This is the first and only child
                phe.last_child = entity.EntityPointer;
                phe.first_child = entity.EntityPointer;
            }
            else
            {
                // Get the last child and set its sibling
                if (world.TryGetEntity(phe.last_child, out var siblingEntity))
                {
                    var she = siblingEntity.Get<HierarchyEntity>();

                    she.next_sibling = entity.EntityPointer;
                    he.previous_sibling = phe.last_child;
                    // Wrap around with the siblings?
                    he.next_sibling = phe.first_child;

                    siblingEntity.Set(she);
                }
            }
            parentEntity.Set(phe);
        }

        entity.Add(he);

        // Add style directly since it can be considered a component
        // TODO. DO not add style directly?! Have a lookup so widget can use common styles
        // Only do per entity for variables that would differ between them
        entity.Add(info.style);

        return entity;
    }

    public Entity AddText(Entity entity, string text)
    {
        entity.Add(new Text(strings.Count));
        strings.Add(text);
        return entity;
    }

    
    public void Render(RendererSpriteSimple renderer)
    {
        var widgets = world.Query(query);
        foreach (var entity in widgets)
        {
            var widget = entity.Get<Widget>();
            var layout = entity.Get<GUILayout>();


            renderer.Draw(new Sprite(0, 48, Saket.Engine.Graphics.Color.White), new Transform2D(layout.x, layout.y, 0, 0, layout.w, layout.h));

        }
    }
}