Procedural City
===============

**Procedural City** is a demo that uses a few tricks to build random nice-looking 3D cities.

----------

### Idea

First, neighborhood blocks are randomly placed in an irregular rectangle grid. 

![Irregular Grid](http://pedroboechat.com/images/ProceduralCity-IrregularGrid.png)
  
Then, building allotments are packed inside blocks using [this bin packing algorithm](https://github.com/juj/RectangleBinPack/). Bin packing uses a heuristic that tries to maximize edge contact for "bins". This leads to better fitting of allotments along block edges. Once blocks are filled, non-edge allotments are discarded.

![Bin Packing](http://pedroboechat.com/images/BinPacking.gif)

Buildings are extruded from allotments and their façades decorated by two pattern matrices. Pattern matrices are picked from one of six achitectural styles (art deco, art noveau, brownstone, chicago old school, international and modernist). One pattern matrix implements simple block extrusion/exclusion to be performed in the façade geometry. Another determines which element (ie.: window, door, column) should be appended where in the façade. Details (ie: headers, rails, firestairs) are randomly selected according to the defined achitectural style. Finally, some common props (ie: chimney, water tank) are scattered around the rooftops. 

![Building Generation](http://pedroboechat.com/images/ProceduralCity-BuildingGeneration.gif)

### Download

[Windows](http://pedroboechat.com/downloads/ProceduralCity.zip)
