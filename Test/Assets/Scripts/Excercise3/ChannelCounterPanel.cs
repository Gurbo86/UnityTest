using System.Text;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using Test.ChannelCounter;


public class ChannelCounterPanel: MonoBehaviour
{
    private string[] quadrant = { "Lower/Left", "Lower/Right", "Upper/Left", "Upper/Right" };

    [SerializeField]
    private List<Texture2D> texture = new List<Texture2D>();
    [SerializeField]
    private Button theButton = null;


    private StringBuilder stringBuilder = new StringBuilder();

    private void StartScanParallelOnRegionInterlockX()
    {

        for (int textureIndex = 0; textureIndex < texture.Count; textureIndex++)
        {
            stringBuilder.AppendLine($"<color=olive> Scanning the texture {texture[textureIndex].name}.</color>");

            // Create the 4 quadrants
            int width = texture[textureIndex].width;
            int height = texture[textureIndex].height;
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            textureRegionIndexLimits[] textureQuadrant = new textureRegionIndexLimits[] {
                new textureRegionIndexLimits(0,         halfWidth,  0,          halfHeight, width),
                new textureRegionIndexLimits(halfWidth, width,      0,          halfHeight, width),
                new textureRegionIndexLimits(0,         halfWidth,  halfHeight, height,     width),
                new textureRegionIndexLimits(halfWidth, width,      halfHeight, height,     width)
            };

            // Initialize search data.
            int batchSize = halfWidth * halfHeight;
            NativeArray<Color32> pixelArray = new NativeArray<Color32>(texture[textureIndex].GetPixels32(), Allocator.Persistent);
            NativeArray<long> rChannelSumsResults = new NativeArray<long>(4, Allocator.Temp, NativeArrayOptions.ClearMemory);
            NativeArray<JobHandle> RChannelCounterJobHandler = new NativeArray<JobHandle>(4, Allocator.Persistent);

            for (int quadrantIndex = 0; quadrantIndex < textureQuadrant.Length; quadrantIndex++)
            {
                rChannelCounterJob parallelAtomicEfficientRChannelCounterJob;

                unsafe
                {
                    parallelAtomicEfficientRChannelCounterJob = new rChannelCounterJob()
                    {
                        pixelData = pixelArray,
                        startX = textureQuadrant[quadrantIndex].textureRegionIndexXStart,
                        endX = textureQuadrant[quadrantIndex].textureRegionIndexXEnd,
                        startY = textureQuadrant[quadrantIndex].textureRegionIndexYStart,
                        endY = textureQuadrant[quadrantIndex].textureRegionIndexYEnd,
                        width = textureQuadrant[quadrantIndex].textureRegionwidth,
                        sumPtr = ((long*)rChannelSumsResults.GetUnsafePtr()) + quadrantIndex
                    };
                }

                RChannelCounterJobHandler[quadrantIndex] = parallelAtomicEfficientRChannelCounterJob.ScheduleParallel(pixelArray.Length, batchSize, default);
            }

            JobHandle.CompleteAll(RChannelCounterJobHandler);

            for (int quadrantIndex = 0; quadrantIndex < rChannelSumsResults.Length; quadrantIndex++)
            {
                stringBuilder.AppendLine($"<color=olive> The sum of r channel for the texture {texture[textureIndex].name} in the quadrant {quadrant[quadrantIndex]} is {(rChannelSumsResults[quadrantIndex] / 255.0f)}</color>");
            }

            for (int i = 0; i < textureQuadrant.Length; i++)
            {
                textureQuadrant[i].DisposeAll();
            }

            stringBuilder.AppendLine();
            pixelArray.Dispose();
            rChannelSumsResults.Dispose();
            RChannelCounterJobHandler.Dispose();
        }


        Debug.Log(stringBuilder.ToString());
        stringBuilder.Clear();
    }

    private void Awake()
    {
        if (theButton != null)
        {
            theButton.onClick.AddListener(StartScanParallelOnRegionInterlockX);
        }
        else
        {
            Debug.LogError("You have to add a button. \n\n\n\n");
        }
    }
}
