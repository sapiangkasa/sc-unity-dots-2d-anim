# ECS 2D Animation

A simple 2D sprite animation frame work for Unity entities using custom shaders.

## Package Contents

- Custom animated sprite shader `/Shader/AnimatedSprite.shadergraph`
- Animator Authoring `/Scripts/AnimatorAuthoring.cs`

## Installation

Refer to Unity's official [Package Manager installation instructions](https://docs.unity3d.com/Manual/upm-ui-install.html).

## Requirements

- Unity 6.0

## Limitations

This framework is best suited for simple characters that does not require too many animation states (e.g character for vampire survivor like games that only require 2-3 states).

As of now there is no plan for expanding the capabilities of this framework as the main purpose of it is for personal study.

## Workflow

### Sprite sheet and the animated shader

The provided `AnimatedShader` uses the texture uv to animate the sprite based on the sprite sheet assigned to the material. There are some requirements for how the sprite sheet needs to be setup.

#### Sprite sheet requirement

- Each frame of the sprite sheet needs to be of the same width and height.
- Each frame of the animation needs to be represented as a sprite. E.g. if an animation require 2 frames of the same sprite, the sprite needs to be duplicated in the sprite sheet.
- All sprites of an animation needs to be in the same row.

Refer to the sample for an example on how the sprite sheet is setup

### Animator Authoring component

The `AnimatorAuthoring` component is an abstract class responsible for overrinding the shader's material properties to animate the sprite.

1. Create a concrete class that inherits from `AnimatorAutoring` and add it to your entity that has the required shader.
2. Setup the `AnimationCount` and `MaxFrameCount` properties. These will initialize the relevant components when the entity is baked.
3. In runtime your character controllers can set the `CurrentAnimationData` to set the current animation to be played. Note that the `FrameCount` property also needs to be set to the actual number of frame of the current animation (e.g. if your jump animation only uses 1 frame then set this property to 1). This is to make sure that the shader does not display empty frames.
4. There is an `OnAnimationEndedEventFlag` enableable component that will be enabled when a non looping animation is completed. You can use this flag to handle animation ended events. An enableable component is used so that it does not introduce a structural change. You should disable this component once you have handled the event in your systems.

## Samples

The package contains a basic sample scene that you can use as a reference on how the animator authoring component can be used.

## Credits

Assets used in the sample scene was created by [Pixel Frog](https://pixelfrog-assets.itch.io/). Please refer to their page for details and licenses of their assets.

### Study materials:

- [Tutorial: SURVIVORS-LIKE w/ Unity DOTS & ECS](https://www.youtube.com/watch?v=cc5l66FwpQ4) by Turbo Makes Games
- [EXTREME PERFORMANCE with Unity DOTS! (ECS, Job System, Burst, Hybrid Game Objects)](https://www.youtube.com/watch?v=4ZYn9sR3btg) by Code Monkey
