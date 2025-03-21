/*
 Name: رؤي محمد لطفي محمد
 Dep: SC
 Id: 2022170162
 Section: 3
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

//include GLM library
using GlmNet;

using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Graphics
{
    class Renderer
    {
        Shader sh;
        uint vertexBufferID;

        //3D Drawing
        mat4 m; //model
        mat4 v; //view
        mat4 p; //projection
        mat4 mvp;
        int shader_model_mat_id;
        int shader_view_mat_id;
        int shader_projection_mat_id;
        uint pants_id;
        public float translationX = 0.0f;
        public float translationY = 0.0f;
        public float translationZ = 0.0f;
        Stopwatch timer = Stopwatch.StartNew();
        Texture pants;
        vec3 triangleCenter;

        // Helper method to generate circle vertices
        private float[] GenerateCircle(float centerX, float centerY, float z, float radius, int segments, vec3 color)
        {
            List<float> circleVerts = new List<float>();
            // Center vertex
            circleVerts.AddRange(new[] { centerX, centerY, z, color.x, color.y, color.z });
            // Perimeter vertices
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)(2 * Math.PI * i / segments);
                float x = centerX + radius * (float)Math.Cos(angle);
                float y = centerY + radius * (float)Math.Sin(angle);
                circleVerts.AddRange(new[] { x, y, z, color.x, color.y, color.z });
            }
            return circleVerts.ToArray();
        }

        // Helper method to combine arrays
        private float[] CombineArrays(params float[][] arrays)
        {
            List<float> result = new List<float>();
            foreach (var arr in arrays)
            {
                result.AddRange(arr);
            }
            return result.ToArray();
        }
        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            pants = new Texture(projectPath + "\\Textures\\Texture.png", 1);
            Gl.glClearColor(0.5f, 0.7f, 1.0f, 1.0f);
            
            float[] verts = {

    // ------------------- Front Face -------------------
    // Upper Front (Yellow Shirt)
    -0.5f,  1.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Top-left
     0.5f,  1.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Top-right
     0.5f,  0.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Mid-right (Yellow)
     0.5f,  0.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Mid-right
    -0.5f,  0.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Mid-left (Yellow)
    -0.5f,  1.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Top-left

    // ------------------- Top Face -------------------
    // Entire Top (Yellow)
    -0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Back-left
     0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Back-right
     0.5f,  1.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Front-right
     0.5f,  1.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Front-right
    -0.5f,  1.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Front-left
    -0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Back-left

     // ------------------- Left Face -------------------
    // Upper Left (Yellow Shirt)
    -0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Top-back
    -0.5f,  1.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Top-front
    -0.5f,  0.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Mid-front (Yellow)
    -0.5f,  0.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Mid-front
    -0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-back (Yellow)
    -0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Top-back

    // ------------------- Back Face -------------------
    // Upper Back (Yellow Shirt)
    -0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Top-left
     0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Top-right
     0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-right (Yellow)
     0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-right
    -0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-left (Yellow)
    -0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Top-left

    // ------------------- Right Face -------------------
    // Upper Right (Yellow Shirt)
     0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Top-back
     0.5f,  1.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Top-front
     0.5f,  0.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Mid-front (Yellow)
     0.5f,  0.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Mid-front
     0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-back (Yellow)
     0.5f,  1.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Top-back

    // Lower Front (Brown Pants)
    -0.5f,  0.0f,  0.3f, 0.65f, 0.16f, 0.16f, // Mid-left
     0.5f,  0.0f,  0.3f, 0.65f, 0.16f, 0.16f, // Mid-right
     0.5f, -0.5f,  0.3f, 0.65f, 0.16f, 0.16f, // Bottom-right
     0.5f, -0.5f,  0.3f, 0.65f, 0.16f, 0.16f, // Bottom-right
    -0.5f, -0.5f,  0.3f, 0.65f, 0.16f, 0.16f, // Bottom-left
    -0.5f,  0.0f,  0.3f, 0.65f, 0.16f, 0.16f, // Mid-left

    

    // Lower Back (Brown Pants) BY3ML AL2
    -0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-left
     0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-right
     0.5f, -0.5f, -0.3f, 1.0f, 1.0f, 0.0f, // Bottom-right
     0.5f, -0.5f, -0.3f, 1.0f, 1.0f, 0.0f, // Bottom-right
    -0.5f, -0.5f, -0.3f, 1.0f, 1.0f, 0.0f, // Bottom-left
    -0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-left*/

   

    // Lower Left (Brown Pants)
    -0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-back
    -0.5f,  0.0f,  0.3f, 1.0f, 1.0f, 0.0f, // Mid-front
    -0.5f, -0.5f,  0.3f, 1.0f, 1.0f, 0.0f, // Bottom-front
    -0.5f,-0.5f,  0.3f, 1.0f, 1.0f, 0.0f, // Bottom-front
    -0.5f,-0.5f, -0.3f, 1.0f, 1.0f, 0.0f, // Bottom-back
    -0.5f,  0.0f, -0.3f, 1.0f, 1.0f, 0.0f, // Mid-back
 
    

    // Lower Right (Brown Pants)
     0.5f,  0.0f, -0.3f, 0.65f, 0.16f, 0.16f, // Mid-back
     0.5f,  0.0f,  0.3f, 0.65f, 0.16f, 0.16f, // Mid-front
     0.5f, -0.5f,  0.3f, 0.65f, 0.16f, 0.16f, // Bottom-front
     0.5f, -0.5f,  0.3f, 0.65f, 0.16f, 0.16f, // Bottom-front
     0.5f, -0.5f, -0.3f, 0.65f, 0.16f, 0.16f, // Bottom-back
     0.5f,  0.0f, -0.3f, 0.65f, 0.16f, 0.16f, // Mid-back

    

    // ------------------- Bottom Face -------------------
    // Entire Bottom (Brown Pants)
    -0.5f, -0.5f, -0.3f, 0.65f, 0.16f, 0.16f, // Back-left
     0.5f, -0.5f, -0.3f, 0.65f, 0.16f, 0.16f, // Back-right
     0.5f, -0.5f,  0.3f, 0.65f, 0.16f, 0.16f, // Front-right
     0.5f, -0.5f,  0.3f, 0.65f, 0.16f, 0.16f, // Front-right
    -0.5f, -0.5f,  0.3f, 0.65f, 0.16f, 0.16f, // Front-left
    -0.5f, -0.5f, -0.3f, 0.65f, 0.16f, 0.16f, // Back-left*/

 
    
        // ------------------- Eyes -------------------
     // Left Eye
     -0.2f,  0.7f,  0.31f, 1.0f, 1.0f, 1.0f, // Top-left
     -0.1f,  0.7f,  0.31f, 1.0f, 1.0f, 1.0f, // Top-right
     -0.1f,  0.6f,  0.31f, 1.0f, 1.0f, 1.0f, // Bottom-right
     -0.1f,  0.6f,  0.31f, 1.0f, 1.0f, 1.0f, // Bottom-right
     -0.2f,  0.6f,  0.31f, 1.0f, 1.0f, 1.0f, // Bottom-left
     -0.2f,  0.7f,  0.31f, 1.0f, 1.0f, 1.0f, // Top-left

     // Right Eye
      0.1f,  0.7f,  0.31f, 1.0f, 1.0f, 1.0f, // Top-left
      0.2f,  0.7f,  0.31f, 1.0f, 1.0f, 1.0f, // Top-right
      0.2f,  0.6f,  0.31f, 1.0f, 1.0f, 1.0f, // Bottom-right
      0.2f,  0.6f,  0.31f, 1.0f, 1.0f, 1.0f, // Bottom-right
      0.1f,  0.6f,  0.31f, 1.0f, 1.0f, 1.0f, // Bottom-left
      0.1f,  0.7f,  0.31f, 1.0f, 1.0f, 1.0f,  // Top-left

      //-----------mouth----------
        -0.25f,  0.37f,  0.31f,  0.0f, 0.0f, 0.0f,  // Left
         0.25f,  0.37f,  0.31f,  0.0f, 0.0f, 0.0f ,  // Right
         // ----------------pupils----------
          0.15f,  0.62f,  0.31f, 0.0f, 0.0f, 0.0f,// Top-left
          -0.18f,  0.62f,  0.31f, 0.0f, 0.0f, 0.0f, // Top-left
        // 0.25f,  0.7f,  0.31f, 0.0f, 0.0f, 0.0f, // Top-right

           // ------------------- Legs -------------------

            // Front Right Leg
         0.2f, -0.6f,  0.2f, 1.0f, 1.0f, 0.0f,
         0.2f, -1.1f,  0.2f, 1.0f, 1.0f, 0.0f,
         0.23f, -0.6f, 0.2f, 1.0f, 1.0f, 0.0f,
         0.23f, -0.6f, 0.2f, 1.0f, 1.0f, 0.0f,
         0.2f, -1.1f,  0.2f, 1.0f, 1.0f, 0.0f,
         0.23f, -1.1f, 0.2f, 1.0f, 1.0f, 0.0f,

        // Front Left Leg
        -0.3f, -0.6f,  0.2f, 1.0f, 1.0f, 0.0f,
        -0.3f, -1.1f,  0.2f, 1.0f, 1.0f, 0.0f,
        -0.33f, -0.6f, 0.2f, 1.0f, 1.0f, 0.0f,
        -0.33f, -0.6f, 0.2f, 1.0f, 1.0f, 0.0f,
        -0.3f, -1.1f,  0.2f, 1.0f, 1.0f, 0.0f,
        -0.33f, -1.1f, 0.2f, 1.0f, 1.0f, 0.0f,




        // Left Tooth (Square)
        -0.1f,  0.35f,  0.32f, 1.0f, 1.0f, 1.0f,  // Top Left
        -0.1f,  0.25f,  0.32f, 1.0f, 1.0f, 1.0f,  // Bottom Left
        -0.05f, 0.25f,  0.32f, 1.0f, 1.0f, 1.0f,  // Top Right
        -0.05f, 0.35f,  0.32f, 1.0f, 1.0f, 1.0f,  // Top Right


// Right Tooth (Square)
0.03f,  0.35f,  0.32f, 1.0f, 1.0f, 1.0f,   // Top Left
0.03f,  0.25f,  0.32f, 1.0f, 1.0f, 1.0f,   // Bottom Left
0.08f,  0.25f,  0.32f, 1.0f, 1.0f, 1.0f,   // Top Right
0.08f,  0.35f,  0.32f, 1.0f, 1.0f, 1.0f,   // Top Right




            };
            // Belt Vertex Data (Position, Color, Texture Coordinates)
            float[] beltVertices = {
    //   X     Y     Z      R   G   B   S   T
    -0.5f,  0.0f,  0.35f,  0f, 0f, 0f,  0.1f, 0.9f,  // Mid-left
     0.5f,  0.0f,  0.35f,  0f, 0f, 0f,   0.9f,  0.9f,  // Mid-right
     0.5f, -0.5f,  0.35f,  0f, 0f, 0f,   0.9f, 0.25f,  // Bottom-right
    -0.5f, -0.5f,  0.35f,  0f, 0f, 0f,  0.0f, 0.25f   // Bottom-left
};

            vertexBufferID = GPU.GenerateBuffer(verts);
            pants_id = GPU.GenerateBuffer(beltVertices);
            //m = new mat4(1);
            triangleCenter = new vec3(0, 0.25f, 0);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            p = glm.perspective(45, 4.0f / 3.0f, 0.1f, 100.0f);
            // View matrix 
            // View matrix: eye, center, up
            v = glm.lookAt(
                new vec3(2, 2, 3),  // Camera position (x, y, z)
                new vec3(0, 0, 0),   // Look at the origin (center of SpongeBob)
                new vec3(0, 1, 0)    // Up vector
            );
            // Model matrix: apply transformations to the model
            m = new mat4(1); //malhash lazma f 3amlna identity matrix
            m = glm.scale(new mat4(1), new vec3(1.25f)); // Double the size
                                                         // Our MVP matrix which is a multiplication of our 3 matrices 
            /* List <mat4> l = new List<mat4>();
             l.Add(m);
             l.Add(v);
             l.Add(p);
             mvp = MathHelper.MultiplyMatrices(l);*/

            sh.UseShader();
            shader_model_mat_id = Gl.glGetUniformLocation(sh.ID, "model_mat");
            shader_view_mat_id = Gl.glGetUniformLocation(sh.ID, "view_mat");
            shader_projection_mat_id = Gl.glGetUniformLocation(sh.ID, "projection_mat");
           

            //Gl.glUniformMatrix4fv(shader_model_mat_id, 1, Gl.GL_FALSE, m.to_array());
            Gl.glUniformMatrix4fv(shader_view_mat_id, 1, Gl.GL_FALSE, v.to_array());
            Gl.glUniformMatrix4fv(shader_projection_mat_id, 1, Gl.GL_FALSE, p.to_array());
            //Get a handle for our "MVP" uniform (the holder we created in the vertex shader)
            // MVP_id = Gl.glGetUniformLocation(sh.ID,"MVP");
            //pass the value of the MVP you just filled to the vertex shader
            //Gl.glUniformMatrix4fv(MVP_id, 1, Gl.GL_FALSE, mvp.to_array());

        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, vertexBufferID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glUniformMatrix4fv(shader_model_mat_id, 1, Gl.GL_FALSE, m.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 30); // yellow body
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 30, 30); // brown pants
           // Gl.glDrawArrays(Gl.GL_QUADS, 60, 4); //black detail
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 60, 12); // eyes
            Gl.glLineWidth(2.0f);  // Makes the line thicker
            Gl.glDrawArrays(Gl.GL_LINE_STRIP, 72, 2); // mouth
            Gl.glPointSize(4.0f);
            Gl.glDrawArrays(Gl.GL_POINTS, 74, 2); // pupils
            Gl.glPointSize(4.0f);
            Gl.glDrawArrays(Gl.GL_LINES, 76, 12); // legs
            Gl.glLineWidth(5.0f);
            Gl.glDrawArrays(Gl.GL_POLYGON, 88, 4); // teeth
            Gl.glDrawArrays(Gl.GL_POLYGON, 92, 4); // teeth*/
            // Gl.glDrawArrays(Gl.GL_TRIANGLES, 78, 4); // 72 vertices (12 per face × 6 faces)
            // Gl.glDrawArrays(Gl.GL_TRIANGLES, 82, 4); // 72 vertices (12 per face × 6 faces)
            // Gl.glDrawArrays(Gl.GL_TRIANGLES, 86, 6); // 72 vertices (12 per face × 6 faces)

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, pants_id);

            Gl.glUniformMatrix4fv(shader_model_mat_id, 1, Gl.GL_FALSE, m.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            pants.Bind();
            Gl.glDrawArrays(Gl.GL_QUADS, 0, 4);

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
        }

        const float rotation_speed = 5.0f;
        float rotationAngle = 0;
        public void Update()
        {
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds / 1000.0f;
            rotationAngle += deltaTime * rotation_speed;

            List<mat4> transformations = new List<mat4>();
            //transformations.Add(glm.translate(new mat4(1), -1 * triangleCenter));
            transformations.Add(glm.rotate(rotationAngle, new vec3(0, 1.0f, 0)));
            transformations.Add(glm.translate(new mat4(1), triangleCenter));
            transformations.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));

            m = MathHelper.MultiplyMatrices(transformations);

            timer.Reset();
            timer.Start();
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
