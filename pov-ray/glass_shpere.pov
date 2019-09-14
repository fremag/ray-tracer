
// [640x480, No AA]
// Width=640
// Height=480
// Antialias=Off

// place all settings of globally influenced features here
global_settings {
    ambient_light 1
}



#declare Camera_0 = camera {perspective angle 60               
                            location  <0.0 , 0 ,-3.0>
                            right     x*image_width/image_height
                            look_at   <0.0 , 0 , 0.0>}
camera{Camera_0}

light_source{< -100, 0, -50> color <1,1,1>}

plane{ <0,1,0>, 0 
       texture{ pigment{ checker color rgb<0,0,0> color rgb<1,1,1>}
                finish { ambient 1}  
                scale 4
              } 
              translate -y
     } 


sphere { <0,0,0>, 1
        material{   
            texture { 
                pigment{ rgbf <1, 1, 1, 0.9> }
                finish { reflection 0.09 specular 0.9 ambient 0 diffuse 0.4  
                }
            } 
            interior{ ior 1.5 } 
        } 
}
