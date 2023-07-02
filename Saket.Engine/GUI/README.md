# Saket.UI

## User Guide

Terminology:

|Term | Explanation|
|-----|-----|
|Primitives| Simple geometric primitives as the output of the UIFramework into the renderer. For now only rectangles, Contains Position, Size and, UV information. |
| Element | User space widget. These can be directly manipulated in code. Contains references to primitives. | 
| ECS | Entity Component System. Responsible for managing changes. |
| Widget | The Algorithm responsible for converting local space coordinates to world space. Input: ScreenSize output List of LayoutElements  |
| LayoutElement | Layouting is done with these. These are maintained by the layouting. |

Example

```xml
<e id="menu">
	<e id="menu_file">File</e>
	<e id="menu_edit">Edit</e>
	<e id="menu_view">View</e>
</e>

<!--A Dockable Window Group. Windows will appear as tabs. -->
<wg>
	<w name="window1">
		<e id="window1_reset">Reset Layout</e>
	</w>
	<w name="window2">
		<e>Hello everyone!<e>
	</w>
</wg>

```

In-code DOM Creation

```csharp
// Define styles
StyleSheet StyleSheet = new StyleSheet();

StyleSheet.Add(
	new (
		new ("menu"), // Selector
		new() // styles
		{
			Width = "100%",
			Height = "32px"
		}
	)
);

document.add(new UIElement("id","class1 class2"))
```

## Implementation Details

<del>First class in-code DOM creation?</del>


Goals for a my GUI framework:
<ul>
<li>Retained mode. Possibility of artist facing editor.</li>

<li>Fast: Data oriented. Fast iteration and update of DOM.</li>
<li>Interfacing with custom Windowing and drawing</li>
<li>Docking </li>
<li>Animation</li>
<li>Simple and powerful layout engine</li>
<li>Thinking about multithreaded operations</li>
<li>Renderer Agnostic</li>
</ul>

Dependencies: Saket.ECS


Data flow:
XML + CSS -- Parse and Spawn --> Widgets -- Spawn --> LayoutElement -- Simple Transform --> Primitives -- gl.subBuffer --> Rendering 

Both a widget & element hierarchy has to be maintained.

Widget Hierarchy. An expensive datastructure to edit.
It's easier to traverse from the leafs to the root since each element has a parent. 
[idiomatic-trees-in-rust](https://rust-leipzig.github.io/architecture/2016/12/20/idiomatic-trees-in-rust/)


### Widgets
In essence widget only provide

custom logic 

Widgets do a couple of thingsingsigs. They spawn multiple primitives.

Handle events

They have a index into the primitive world where children primitives will be placed relative to.



#### Text Widget
sad


### Layouting Details
Constraints being passed down to children. Geometry being passed up to parents

Layouting starts from a root node with a fixed size. Then 






### Styling

how styling should be stored and applied

### Events
The event handler. OnElementHover, OnElementEnter, OnElementExit. OnScroll
a stack of events. 
Collision detection?
Can be consumed by any api

### Docking
Docking and Windowing are provider independent/agnostic.
Window element ```<Window>```. Layouting bounderies between Windows can be made since content does not interact with each other.


## TODO

Layouting Algorithm

Event Handling

XML Widget File format 







<br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/>

## Reference Material



https://docs.flutter.dev/resources/inside-flutter

https://www.alibabacloud.com/blog/exploration-of-the-flutter-rendering-mechanism-from-architecture-to-source-code_597285



Data Oriented GUI in Rust by Raph Levien - Bay Area Rust Meetup:
https://www.youtube.com/watch?v=4YTfxresvS8&t=1940s&ab_channel=Rust

Rust + WGPU - Simple -Single draw call- GUI showcase:
https://www.youtube.com/watch?v=gUmCIys6vsE&ab_channel=EuriGilbertoHerasmeCuevas
https://t.co/qBwYKAAJpa


CSS Flexible Box Layout Module Level 1:
https://www.w3.org/TR/css-flexbox-1/


https://github.com/NXPmicro/gtec-demo-framework/blob/master/Doc/FslSimpleUI.md


Our machinery Blog on UI: 
One Draw Call UI: https://web.archive.org/web/20220702205403/https://ourmachinery.com/post/one-draw-call-ui/

UI rendering using Primitive Buffers : https://web.archive.org/web/20210831125334/https://ourmachinery.com/post/ui-rendering-using-primitive-buffers/


A 7+1 part series on Immediate mode GUI by Ryan Fleury
https://ryanfleury.substack.com/


https://raphlinus.github.io/personal/2018/05/08/ecs-ui.html


https://www.w3.org/TR/CSS2/visuren.html#normal-flow

###  A list of UI libraries in different languages:

#### C
https://github.com/Immediate-Mode-UI/Nuklear

#### C++
https://github.com/idea4good/GuiLite
https://github.com/ocornut/imgui
https://github.com/hikogui/hikogui
http://cegui.org.uk/
http://mygui.info/

#### C#
https://github.com/AvaloniaUI/Avalonia
https://github.com/wieslawsoltes/Dock
https://github.com/migueldeicaza/gui.cs
https://github.com/rds1983/Myra
https://github.com/RonenNess/GeonBit.UI
https://github.com/Apostolique/Apos.Gui
https://github.com/Roderik11/Squid

#### Rust
https://github.com/iced-rs/iced
https://github.com/fschutt/azul

#### Go
https://github.com/fyne-io/fyne



### Layouting
https://github.com/facebook/yoga

https://proandroiddev.com/understanding-flutter-layout-box-constraints-292cc0d5e807
https://www.youtube.com/watch?v=UUfXWzp0-DU


A high performance Rust-powered layout library:
https://github.com/DioxusLabs/taffy

https://subformapp.com/articles/why-not-flexbox/
https://github.com/lynaghk/subform-layout
https://github.com/vizia/morphorm
https://www.deconstructconf.com/2017/kevin-lynagh-choosing-features

https://github.com/randrew/layout
https://github.com/nicklockwood/layout

https://geom3trik.github.io/tuix-book/layout/stack_overview.html

HTML RENDERERS
https://github.com/lexbor/lexbor
https://github.com/lexborisov/modest
https://github.com/litehtml/litehtml
https://github.com/ultralight-ux/ultralight