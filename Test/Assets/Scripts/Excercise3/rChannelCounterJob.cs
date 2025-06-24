using System.Threading;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Test.ChannelCounter
{
    public struct rChannelCounterJob : IJobFor
    {
        [ReadOnly] public NativeArray<Color32> pixelData;
        [ReadOnly] public NativeReference<int> startX;
        [ReadOnly] public NativeReference<int> endX;
        [ReadOnly] public NativeReference<int> startY;
        [ReadOnly] public NativeReference<int> endY;
        [ReadOnly] public NativeReference<int> width;

        [NativeDisableUnsafePtrRestriction]
        unsafe public long* sumPtr;

        public void Execute(int index)
        {
            int height = index / width.Value;

            if ((height >= startY.Value) && (height < endY.Value))
            {
                int displacement = height * (width.Value);
                int firstPosition = startX.Value + displacement;
                int lastPosition = endX.Value + displacement;

                if ((index >= firstPosition) && (index < lastPosition))
                {
                    unsafe
                    {
                        Interlocked.Add(ref *sumPtr, pixelData[index].r);
                    }
                }
            }
        }
    }

    struct textureRegionIndexLimits
    {
        public NativeReference<int> textureRegionIndexXStart;
        public NativeReference<int> textureRegionIndexXEnd;
        public NativeReference<int> textureRegionIndexYStart;
        public NativeReference<int> textureRegionIndexYEnd;
        public NativeReference<int> textureRegionwidth;

        public textureRegionIndexLimits(int startX, int endX, int startY, int endY, int width)
        {
            textureRegionIndexXStart = new NativeReference<int>(startX, Allocator.Persistent);
            textureRegionIndexXEnd = new NativeReference<int>(endX, Allocator.Persistent);
            textureRegionIndexYStart = new NativeReference<int>(startY, Allocator.Persistent);
            textureRegionIndexYEnd = new NativeReference<int>(endY, Allocator.Persistent);
            textureRegionwidth = new NativeReference<int>(width, Allocator.Persistent);
        }

        public void DisposeAll()
        {
            textureRegionIndexXStart.Dispose();
            textureRegionIndexXEnd.Dispose();
            textureRegionIndexYStart.Dispose();
            textureRegionIndexYEnd.Dispose();
            textureRegionwidth.Dispose();
        }
    }
}
