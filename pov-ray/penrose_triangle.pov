// http://www.f-lohmueller.de/pov_tut/x_sam/sam_430f.htm
// create a regular point light source
light_source {
  0*x                  // light's position (translated below)
  color rgb <1,1,1>    // light's color
  translate <-20, -10, -20>
}


camera{ //-------------------------- 
   //orthographic
   location  < 0, 0, -20>
   right  x*image_width/image_height
   angle    5
   look_at <0,0,0>
} //----------

//-----------------------------------
#declare Length   = 1;
#declare Diameter = 0.15;
// internals:
#declare R = Diameter/2;
#declare L = Length - 2*R;
//-----------------------------------
#declare Triangle_Texture =
texture{ pigment{ color rgb<1,.9,.3>}
         finish { phong 1 }
       } // end of texture
//-----------------------------------
#declare Element =
box { <-R,-R,-R>,< L+R, R, R>
      texture {Triangle_Texture}
      //no_shadow
    } // end of box
//-----------------------------------
#declare Element_Cut =
intersection{
 object{ Element }
 plane{ <1,0,0>,0
        rotate<0,45,0>
        translate<L/2,0,0>
        texture {Triangle_Texture} }
} // end difference
//-----------------------------------
#declare Penrose_Triangle =
union{
object{ Element_Cut rotate<0,-90, 0>}
object{ Element     rotate<0,  0,90>}
object{ Element     rotate<0,  0, 0>              
      translate<0,L,0>
      }
 object{ Element_Cut rotate<0, 90, 0>                    translate<L,L,0>}
no_shadow
} // end of union //----------------- 


object {
    Penrose_Triangle   
    rotate <-45, 35, 0>
    translate -L/2*y
}                    
