//
// SPDX-License-Identifier: CC0-1.0
//
// This example code file is released to the public under Creative Commons CC0.
// See https://creativecommons.org/publicdomain/zero/1.0/legalcode
//
// To the extent possible under law, LEAP 71 has waived all copyright and
// related or neighboring rights to this LEAP 71 ShapeKernel Example Code.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//


using PicoGK;
using System.Numerics;


namespace Leap71
{
    using ShapeKernel;

    namespace ShapeKernelExamples
    {
        class BasePipeSegmentShowCase
        {
            public static void Task()
            {
                try
                {
                    {
                        //basic
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, 0, 0));
                        BasePipeSegment oShape  = new BasePipeSegment(oLocalFrame, 60, 20, 40,
                            new LineModulation(MathF.PI),
                            new LineModulation(0.5f * MathF.PI),
                            BasePipeSegment.EMethod.MID_RANGE);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrBlue);
                    }

                    {
                        //modulated 0
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, 50, 0));
                        BasePipeSegment oShape  = new BasePipeSegment(oLocalFrame, 60, 2, 40,
                            new LineModulation(MathF.PI),
                            new LineModulation(fGetSegmentPhiRange2),
                            BasePipeSegment.EMethod.MID_RANGE);
                        oShape.SetLengthSteps(500);
                        oShape.SetRadius(
                            new SurfaceModulation(6f),
                            new SurfaceModulation(new LineModulation(fGetLineModulation1)));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrCrystal);
                    }

                    {
                        //modulated 1
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(0, -50, 0));
                        BasePipeSegment oShape  = new BasePipeSegment(oLocalFrame, 60, 2, 40,
                            new LineModulation(MathF.PI),
                            new LineModulation(fGetSegmentPhiRange1),
                            BasePipeSegment.EMethod.MID_RANGE);
                        oShape.SetLengthSteps(500);
                        oShape.SetRadius(
                            new SurfaceModulation(fGetSurfaceModulation3),
                            new SurfaceModulation(fGetSurfaceModulation1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrYellow);
                    }

                    {
                        //modulated 2
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(50, -50, 0));
                        BasePipeSegment oShape  = new BasePipeSegment(oLocalFrame, 60, 2, 40,
                            new LineModulation(fGetSegmentPhiMid1),
                            new LineModulation(1.75f * MathF.PI),
                            BasePipeSegment.EMethod.MID_RANGE);
                        oShape.SetLengthSteps(500);
                        oShape.SetRadius(
                            new SurfaceModulation(fGetSurfaceModulation4),
                            new SurfaceModulation(fGetSurfaceModulation2));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrRuby);
                    }

                    {
                        //modulated + spined
                        ISpline oSpine          = new ExampleSpline();
                        Frames aFrames          = new Frames(oSpine.aGetPoints(), Vector3.UnitY);
                        BasePipeSegment oShape  = new BasePipeSegment(aFrames, 2, 40,
                            new LineModulation(fGetSegmentPhiMid2),
                            new LineModulation(1.5f * MathF.PI),
                            BasePipeSegment.EMethod.MID_RANGE);
                        oShape.SetRadius(
                            new SurfaceModulation(fGetSurfaceModulation3),
                            new SurfaceModulation(fGetSurfaceModulation1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrRacingGreen);
                    }

                    {
                        //spined
                        ISpline oSpine          = new ExampleSpline();
                        List<Vector3> aPoints   = SplineOperations.aTranslateList(oSpine.aGetPoints(), 50 * Vector3.UnitX);
                        Frames aFrames          = new Frames(aPoints, Vector3.UnitY);
                        BasePipeSegment oShape  = new BasePipeSegment(aFrames, 10, 12,
                            new LineModulation(fGetSegmentPhiMid1),
                            new LineModulation(1f * MathF.PI),
                            BasePipeSegment.EMethod.MID_RANGE);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrGray);
                    }
                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }


            //functions for pipe segment line modulations
            protected static float fGetSegmentPhiMid1(float fLengthRatio)
            {
                float fPhiMid = -1f * MathF.PI * fLengthRatio;
                return fPhiMid;
            }

            protected static float fGetSegmentPhiMid2(float fLengthRatio)
            {
                float fPhiMid = 4f * MathF.PI * fLengthRatio;
                return fPhiMid;
            }

            protected static float fGetSegmentPhiRange1(float fLengthRatio)
            {
                float fPhiRange = 0.5f * MathF.PI + 0.25f * MathF.PI * MathF.Cos(8f * fLengthRatio);
                return fPhiRange;
            }

            protected static float fGetSegmentPhiRange2(float fLengthRatio)
            {
                float fPhiRange = 0.5f * MathF.PI + 0.25f * MathF.PI * MathF.Cos(40f * fLengthRatio);
                return fPhiRange;
            }

            //functions for generic line modulations
            public static float fGetLineModulation1(float fLengthRatio)
            {
                float fWidth = 10f - 3f * MathF.Cos(8f * fLengthRatio);
                return fWidth;
            }

            //functions for generic surface modulations
            public static float fGetSurfaceModulation1(float fPhi, float fLengthRatio)
            {
                float fRadius = 12f + 3f * MathF.Cos(5f * fPhi);
                return fRadius;
            }

            public static float fGetSurfaceModulation2(float fPhi, float fLengthRatio)
            {
                fPhi += 1f * MathF.PI * fLengthRatio;
                float fRadius = 10f - 2f * MathF.Cos(3f * fPhi) + 9f * fLengthRatio;
                return fRadius;
            }

            public static float fGetSurfaceModulation3(float fPhi, float fLengthRatio)
            {
                float fRadius = 8f + 5f * MathF.Cos(5f * fPhi);
                return fRadius;
            }

            public static float fGetSurfaceModulation4(float fPhi, float fLengthRatio)
            {
                fPhi += 1f * MathF.PI * fLengthRatio;
                float fRadius = 9f - 1f * MathF.Cos(3f * fPhi) + 7f * fLengthRatio;
                return fRadius;
            }
        }
    }
}