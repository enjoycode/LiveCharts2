// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Motion;

namespace LiveChartsCore.Drawing;

/// <inheritdoc cref="IAnimatable" />
public abstract class Animatable : IAnimatable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Animatable"/> class.
    /// </summary>
    protected Animatable() { }

    /// <inheritdoc cref="IAnimatable.IsValid" />
    public bool IsValid { get; set; } = true;

    /// <inheritdoc cref="IAnimatable.CurrentTime" />
    public long CurrentTime { get; set; } = long.MinValue;

    /// <inheritdoc cref="IAnimatable.RemoveOnCompleted" />
    public bool RemoveOnCompleted { get; set; }

    /// <inheritdoc cref="IAnimatable.MotionProperties" />
#if __WEB__
    public StringMap<IMotionProperty> MotionProperties { get; } = new();
#else
    public Dictionary<string, IMotionProperty> MotionProperties { get; } = new();
#endif

    /// <inheritdoc cref="IAnimatable.SetTransition(Animation?, string[])" />
    public void SetTransition(Animation? animation, params string[]? propertyName)
    {
#if __WEB__
        var a = animation?.Duration == 0L ? null : animation;
        if (propertyName is null || propertyName.Length == 0) propertyName = MotionProperties.Keys.ToArray();

        foreach (var name in propertyName)
        {
            MotionProperties.get(name)!.Animation = a;
        }
#else
        var a = animation?.Duration == 0 ? null : animation;
        if (propertyName is null || propertyName.Length == 0) propertyName = MotionProperties.Keys.ToArray();

        foreach (var name in propertyName)
        {
            MotionProperties[name].Animation = a;
        }
#endif
    }

    /// <inheritdoc cref="IAnimatable.RemoveTransition(string[])" />
    public void RemoveTransition(params string[]? propertyName)
    {
#if __WEB__
        if (propertyName is null || propertyName.Length == 0) propertyName = MotionProperties.Keys.ToArray();

        foreach (var name in propertyName)
        {
            MotionProperties.get(name)!.Animation = null;
        }
#else
        if (propertyName is null || propertyName.Length == 0) propertyName = MotionProperties.Keys.ToArray();

        foreach (var name in propertyName)
        {
            MotionProperties[name].Animation = null;
        }
#endif
    }

    /// <inheritdoc cref="IAnimatable.CompleteTransition(string[])" />
    public virtual void CompleteTransition(params string[]? propertyName)
    {
#if __WEB__
        if (propertyName is null || propertyName.Length == 0) propertyName = MotionProperties.Keys.ToArray();

        foreach (var property in propertyName)
        {
            var transitionProperty = MotionProperties.get(property);
            if (transitionProperty == null)
                throw new Exception(
                    $"The property {property} is not a transition property of this instance.");

            if (transitionProperty.Animation is null) continue;
            transitionProperty.IsCompleted = true;
        }
#else
        if (propertyName is null || propertyName.Length == 0) propertyName = MotionProperties.Keys.ToArray();

        foreach (var property in propertyName)
        {
            if (!MotionProperties.TryGetValue(property, out var transitionProperty))
                throw new Exception(
                    $"The property {property} is not a transition property of this instance.");

            if (transitionProperty.Animation is null) continue;
            transitionProperty.IsCompleted = true;
        }
#endif
    }

    /// <summary>
    /// Registers a motion property.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="motionProperty">The transition.</param>
    /// <returns></returns>
    protected T RegisterMotionProperty<T>(T motionProperty)
        where T : IMotionProperty
    {
#if __WEB__
        MotionProperties.set(motionProperty.PropertyName, motionProperty);
        return motionProperty;
#else
        MotionProperties[motionProperty.PropertyName] = motionProperty;
        return motionProperty;
#endif
    }
}
