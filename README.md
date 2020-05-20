# ray-tracer
[The Ray Tracer Challenge](https://pragprog.com/book/jbtracer/the-ray-tracer-challenge)

<details>
<summary>Ambiguous cylinder</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/ambiguous_cylinder.png"/>
Thanks to TJ Wei for his <a href="https://www.youtube.com/watch?v=vUz4kov3K1c">video</a>
</details>

<details>
<summary>Triangle optimizations</summary>
 <table>
 <tr><th> Scene   </th><th>Standard</th><th> Vector3  </th><th> Inline  </th><th> AVX 256 </th></tr> 
 <tr> <td>Dragons   </td><td>22 s</td><td>    17 s      </td><td>     7 s      </td><td>   3 s    </td></tr>
 <tr> <td>Christmas    </td><td>11 s</td><td>      9 s     </td><td>     4 s     </td><td>  3 s    </td></tr>
 </table>
</details>

<details>
 <summary>Bonus chapter: <a href="http://www.raytracerchallenge.com/bonus/bounding-boxes.html">Bounding Volume hierarchy</a></summary><img src="https://github.com/fremag/ray-tracer/blob/master/demos/dragon_volume_hierarchy.png"/>
<br/>
Reference:
<br/>
<img src="http://www.raytracerchallenge.com/bonus/images/bbox/bounding-boxes.jpg"/>
<table>
<tr><th> BVH   </th><th> #Tri. Inters.  </th><th> # Box Inters.  </th><th> Time (1 thread) </th></tr> 
<tr> <td>No   </td><td>  7512 M        </td><td>   2.6 M        </td><td>   325 s    </td></tr>
<tr> <td>Yes    </td><td>   347 M        </td><td>  14.3 M        </td><td>  15 s    </td></tr>
<tr><th> Ratio </th><th>   x 22         </th><th>  / 5.5      </th><th>  x 22    </th></tr>
</table>
</details>

<details>
  <summary>Iso surface</summary>
  <a href="https://en.wikipedia.org/wiki/Marching_cubes">marching cubes</a> / <a href="https://en.wikipedia.org/wiki/Metaballs">Metaballs</a>
  <br/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/isosurface_smooth.png"/>
  <br/>
  <br/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/isosurface_basic_shapes.png"/>
  <br/>
  Thanks to <a href="https://github.com/Scrawk/Marching-Cubes">Scrawk</a> for his code.
</details>

<details>
  <summary>Christmas</summary>
<img src="https://github.com/fremag/ray-tracer/blob/master/demos/christmas.png"/>
<br/>
<a href="https://forum.raytracerchallenge.com/thread/16/merry-christmas-scene-description?page=1&scrollTo=697">Reference:</a>
<br/>
<img src="https://i.imgur.com/QKBOAQW.jpg"/>
</details>

<details>
  <summary>Orthographic camera</summary>
  <a href="https://en.wikipedia.org/wiki/Penrose_triangle">Penrose triangle</a><br/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/penrose_triangle.png"/>
</details>

<details>
  <summary>Perlin noise / pattern</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/the_one_ring_perlin.png"/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/perlin_ring_expanse.png"/>
</details>

<details>
  <summary>Spot lights</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/spot_lights.png"/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/spot_light_soft_shadow.png"/>
</details>

<details>
  <summary>Bonus Chapter: soft shadows</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/soft_shadows.png"/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/shadow_glamour_shot.png"/>
<p>Reference:
  <img src="http://www.raytracerchallenge.com/bonus/images/shadow-glamour-shot.jpg"/>
</p>
</details>

<details>
  <summary>Chapter xx: demos</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/menger_castle.png"/>
</details>

<details>
  <summary>Chapter xx: basic UI</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/demo-ui.gif"/>

</details>

<details>
  <summary>Chapter xx: more triangles</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/curve_sweep.png"/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/height_field.png"/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/wireframe.png"/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/prism.png"/>
</details>

<details>
  <summary>Chapter 16: Constructive Solid Geometry (CSG)</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/menger_sponge.png"/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/csg.png"/>
</details>

<details>
  <summary>Chapter 15: triangles </summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/teapot.png"/>
  <p>Smooth triangles</p>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/pikachu.png"/>
</details>

<details>
  <summary>Chapter 14: groups </summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/labyrinth.png"/>
</details>

<details>
  <summary>Chapter 13: cylinders </summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/cylinders_altitude.png"/>
  <br/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/cones.png"/>
</details>

<details>
  <summary>Chapter 12: cubes </summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/cubes.png"/>
</details>

<details>
  <summary>Pov-ray ? </summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/glass_sphere.png"/>
  <br/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/pov-ray/glass_sphere.png"/>
</details>

<details>
  <summary>Chapter 11: Reflection and Refraction</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/world_reflection_refraction.png"/>
  <br/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/glass_sphere.png"/>
  <br/>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/world_reflection.png"/>
  <p>
         Thanks to <a href="https://github.com/javan">Javan Makhmali</a>
         I wanted to check my ray tracer was correct so I got the <a href="https://github.com/javan/ray-tracer-challenge/blob/master/src/controllers/chapter_11_worker.js">same scene as him</a> to compare results.
  </p>
</details>

<details>
  <summary>Chapter 10: patterns</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/world_patterns.png"/>
</details>

<details>
  <summary>Chapter 9: planes</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/world_plane.png"/>
</details>

<details>
  <summary>Chapter 8: shadows</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/helloworld_shadows.png"/>
</details>

<details>
  <summary>Chapter 8: Shadows with "acne" issue</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/helloworld_shadow_acne.png"/>
</details>

<details>
  <summary>Chapter 7: making a scene</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/helloworld.png"/>
</details>

<details>
  <summary>Chapter 6: lighting and shading</summary>
  <img src="https://github.com/fremag/ray-tracer/blob/master/demos/sphere.png"/>
</details>

