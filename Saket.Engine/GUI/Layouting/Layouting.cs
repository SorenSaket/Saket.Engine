﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using Saket.ECS;
using Saket.Engine.GUI.Components;
using Saket.Engine.GUI.Styling;




namespace Saket.Engine.GUI.Layouting;

// Input: of the layouting should be the elements
// Output: another type of element? with absolute positions only????+
public class Layouting
{
    //static Query query = new Query().With<(Widget, HierarchyEntity, GUILayout, Style)>();


    // temp storage for calculations
   // HashSet<ECSPointer> children;


    //Stack<ECSPointer> stack;
    //Queue<ECSPointer> queue;

    public void Layout(World world)
    {
        // TODO get all roots

        // iterate roots
        // layout
       // LayoutEntity();
    }


    // Dirty == style has changed (Only the values applicable to layouting) && tree changed (child added/removed)
    // If width/height is unset the entity will size to fit its content


    // Before going to layouting we have a collection of dirty entities that have been modified
    // When an entitiy that is dirty get modified outer sizing
    // the parent & children should also become dirty and recalculated
    // We want to do as little calculations as possible
    // The function has to be recursive in some fasion
    // The goal of the function is to determine its own GUILayout. aka width, height, x, y
    



    // Note: The layout direction can be either horizontal or vertical
    // So only one axis. A dimension on this axis will be called a size From here on out.
    //  1: Determine inner size 
    //      1.1: size-padding
    //      1.2: Add up all fixed sized children and margin (% and px)
    //  If theres only one stretch child fill it out. 
    //  2: Get the min size of all stretch children.
    //      
    //      2.2: Go down the tree and and repeat this recursively? (Try to avoid actual recursion?)
    //  3: Distribute the leftover space between the strech children based on factor


    // Iterate all children and combine their widths/heights

    // requested sizes

    struct LayoutResult
    {
        public Vector2 preferedSize;
        public bool changed;//?
    }
    
    // Return bool whether the entity changed size or not? 
   /* public static LayoutResult LayoutEntity(Entity entity, in Constraints constraints)
    {
        var h       =  entity.Get<HierarchyEntity>();
        var style   =  entity.Get<Style>();
        var layout  =  entity.Get<GUILayout>();

        float innerWidth = 0;
        float innerHeight = 0;

        // If width and height are static set immediately
        if (style.Width.Measurement == Measurement.Pixels)
            layout.w = style.Width.Value;
        else if (style.Width.Measurement == Measurement.Percentage)
            layout.w = constraints.MaxWidth * style.Width.Value * 0.01f;



        // Apply contraints
        //ApplyConstraints(ref innerWidth, ref innerHeight, constraints);


        // TODO this is assumed to always be in pixels
        innerSize -= style.innerSpacing.Value;

        //

        // The leftover space for stretch children
        float mainAxisSize = innerSize;
        float rowLeftover = mainAxisSize;
        int rows = 1;


        // Fixed
        foreach (var child in HierarchyEntityChildIterator(entity))
        {
            // Get unconstrained prefered Size 
            LayoutEntity(child, default);
            ref var childlayout = ref child.GetRef<GUILayout>();

            rowLeftover -= childlayout.w;

            if (style.Wrap)
            {
                if(rowLeftover < 0)
                {
                    rows++;
                }
            }
        }

        // Acutally perform layout
        foreach (var child in HierarchyEntityChildIterator(entity))
        {
            // Get unconstrained prefered Size 
            LayoutEntity(child, default);
            
        }

        // Apply the layout
        entity.Set(layout);
    }
    */







    /// <summary>
    /// It it assumed a constaint value of 0 means that no constraint should be applied
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="Constraints"></param>
    
    static void ApplyConstraints(ref float width, ref float height, in Constraints Constraints)
    {
        width = Constraints.MaxWidth != 0 ? Mathf.Min(width, Constraints.MaxWidth) : width;
        width = Constraints.MinWidth != 0 ? Mathf.Max(width, Constraints.MinWidth) : width;
        height = Constraints.MaxHeight != 0 ? Mathf.Min(height, Constraints.MaxHeight) : height;
        height = Constraints.MinHeight != 0 ? Mathf.Max(height, Constraints.MinHeight) : height;
    }

    /// <summary>
    /// An iterator that returns all children in order of chain
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public static IEnumerable<Entity> HierarchyEntityChildIterator(Entity root)
    {
        Entity currentEntity;
        HierarchyEntity current = root.Get<HierarchyEntity>();

        // If the root has a first child
        if (current.first_child != default)
        {
            currentEntity = new Entity(root.World, current.first_child);
            current = currentEntity.Get<HierarchyEntity>();
            yield return currentEntity;
        }
        else
        {
            // The root doesn't have any children
            yield break;
        }

        while (current.next_sibling != default)
        {
            currentEntity = new Entity(root.World, current.next_sibling);
            current = currentEntity.Get<HierarchyEntity>();
            yield return currentEntity;
        }

    }

    public static IEnumerable<Entity> HierarchyEntityBreathFirstIterator(Entity root, Queue<Entity> queue)
    {
        queue.Enqueue(root);

        Entity currentEntity;
        HierarchyEntity current;

        while (queue.TryDequeue(out currentEntity))
        {
            current = currentEntity.Get<HierarchyEntity>();

            // Go down in the tree
            // If the root has a first child
            if (current.first_child != default)
            {
                currentEntity = new Entity(root.World, current.first_child);
                current = currentEntity.Get<HierarchyEntity>();
                queue.Enqueue(currentEntity);
                yield return currentEntity;
            }

            // Go through the entire breath
            while (current.next_sibling != default)
            {
                currentEntity = new Entity(root.World, current.next_sibling);
                current = currentEntity.Get<HierarchyEntity>();
                queue.Enqueue(currentEntity);
                yield return currentEntity;
            }
        } 

    }

    public static IEnumerable<Entity> HierarchyEntityDepthFirstIterator(Entity root, Stack<Entity> stack)
    {
        stack.Push(root);

        Entity currentEntity;
        HierarchyEntity current;

        while (stack.TryPop(out currentEntity))
        {
            current = currentEntity.Get<HierarchyEntity>();

            // Go down in the tree
            // If the root has a first child
            if (current.first_child != default)
            {
                currentEntity = new Entity(root.World, current.first_child);
                current = currentEntity.Get<HierarchyEntity>();
                stack.Push(currentEntity);
                yield return currentEntity;
            }

            // Go through the entire breath
            while (current.next_sibling != default)
            {
                currentEntity = new Entity(root.World, current.next_sibling);
                current = currentEntity.Get<HierarchyEntity>();
                stack.Push(currentEntity);
                yield return currentEntity;
            }
        }
    }

}