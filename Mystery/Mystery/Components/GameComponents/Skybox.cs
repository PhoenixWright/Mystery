using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mystery.Components.EngineComponents;

namespace Mystery.Components.GameComponents
{
  public class Skybox : Component
  {
    // Skybox
    Camera3D camera;
    Effect effect;
    Texture2D[] skyboxTextures;
    Model skyboxModel;

    public Vector3 CameraTarget { get; set; }

    public Skybox(Engine engine) : base(engine) {
      CameraTarget = Vector3.Forward;
      camera = new Camera3D(engine);
      camera.Target = new Vector3(1, 0, 0);

      effect = engine.Content.Load<Effect>(@"Shaders\Effects");
      skyboxModel = LoadModel(@"Skyboxes\Space", out skyboxTextures);

      Engine.AddComponent(this);
    }

    public override void Update(GameTime gameTime)
    {
      // this gets pretty close to what we want, camera target just needs to go around the cam at 0, 0, 0
      camera.Target = CameraTarget;

      base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      DrawSkybox();

      base.Draw(gameTime);
    }

    public override void UnloadContent()
    {
      base.UnloadContent();
    }

    private Model LoadModel(string assetName, out Texture2D[] textures)
    {
      Model newModel = Engine.Content.Load<Model>(assetName);
      textures = new Texture2D[newModel.Meshes.Count];
      int i = 0;
      foreach (ModelMesh mesh in newModel.Meshes) {
        foreach (BasicEffect currentEffect in mesh.Effects) {
          textures[i++] = currentEffect.Texture;
        }
      }

      foreach(ModelMesh mesh in newModel.Meshes) {
        foreach(ModelMeshPart meshPart in mesh.MeshParts) {
          meshPart.Effect = effect.Clone();
        }
      }

      return newModel;
    }

    private void DrawSkybox()
    {
      SamplerState ss = new SamplerState();
      ss.AddressU = TextureAddressMode.Clamp;
      ss.AddressV = TextureAddressMode.Clamp;

      Engine.Video.GraphicsDevice.SamplerStates[0] = ss;

      DepthStencilState dss = new DepthStencilState();
      dss.DepthBufferEnable = false;
      Engine.Video.GraphicsDevice.DepthStencilState = dss;

      Matrix[] skyboxTransforms = new Matrix[skyboxModel.Bones.Count];
      skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransforms);

      int i = 0;
      foreach(ModelMesh mesh in skyboxModel.Meshes) {
        foreach(Effect currentEffect in mesh.Effects) {
          Matrix worldMatrix = skyboxTransforms[mesh.ParentBone.Index];
          currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
          currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
          currentEffect.Parameters["xView"].SetValue(camera.ViewMatrix);
          currentEffect.Parameters["xProjection"].SetValue(camera.ProjectionMatrix);
          currentEffect.Parameters["xTexture"].SetValue(skyboxTextures[i++]);
        }

        mesh.Draw();
      }

      dss = new DepthStencilState();
      dss.DepthBufferEnable = true;
      Engine.Video.GraphicsDevice.DepthStencilState = dss;
    }
  }
}
