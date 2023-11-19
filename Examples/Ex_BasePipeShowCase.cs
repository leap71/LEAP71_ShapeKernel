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


namespace Leap71
{
    using System.Numerics;
    using ShapeKernel;

    namespace ShapeKernelExamples
    {
        class BasePipeShowCase
        {
            public static void Task()
            {
                try
                {
                    {
                        //basic
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, 0, 0));
                        BasePipe oShape         = new BasePipe(oLocalFrame, 60, 10, 20);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrBlue);
                    }

                    {
                        //transformed
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(0, 0, 0));
                        BasePipe oShape         = new BasePipe(oLocalFrame, 60, 10, 20);
                        oShape.SetTransformation(vecGetTransformation);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrGreen);
                    }

                    {
                        //modulated 0
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(50, -50, 0));
                        BasePipe oShape         = new BasePipe(oLocalFrame, 60, 2, 40);
                        oShape.SetLengthSteps(500);
                        oShape.SetRadius(
                            new SurfaceModulation(6f),
                            new SurfaceModulation(new LineModulation(fGetLineModulation1)));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrLemongrass);
                    }

                    {
                        //modulated 1
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, -50, 0));
                        BasePipe oShape         = new BasePipe(oLocalFrame, 60, 2, 40);
                        oShape.SetLengthSteps(500);
                        oShape.SetRadius(
                            new SurfaceModulation(fGetSurfaceModulation3),
                            new SurfaceModulation(fGetSurfaceModulation1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrLavender);
                    }

                    {
                        //modulated 2
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(0, -50, 0));
                        BasePipe oShape         = new BasePipe(oLocalFrame, 60, 2, 40);
                        oShape.SetLengthSteps(500);
                        oShape.SetRadius(
                            new SurfaceModulation(fGetSurfaceModulation4),
                            new SurfaceModulation(fGetSurfaceModulation2));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrRed);
                    }

                    {
                        //modulated + spined
                        ISpline oSpine          = new ExampleSpline();
                        Frames aFrames          = new Frames(oSpine.aGetPoints(), Vector3.UnitY);
                        BasePipe oShape         = new BasePipe(aFrames, 2, 40);
                        oShape.SetRadius(
                            new SurfaceModulation(fGetSurfaceModulation3),
                            new SurfaceModulation(fGetSurfaceModulation1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrOrchid);
                    }

                    {
                        //spined
                        ISpline oSpine          = new ExampleSpline();
                        List<Vector3> aPoints   = SplineOperations.aTranslateList(oSpine.aGetPoints(), 50 * Vector3.UnitX);
                        Frames aFrames          = new Frames(aPoints, Vector3.UnitY);
                        BasePipe oShape         = new BasePipe(aFrames, 10, 12);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrToothpaste);
                    }
                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }

            //functions for generic line modulations
            public static float fGetLineModulation1(float fLengthRatio)
            {
                float fWidth = 10f - 3f * MathF.Cos(8f * fLengthRatio);
                return fWidth;
            }

            public static float fGetLineModulation2(float fLengthRatio)
            {
                float fDepth = 8f - 1f * MathF.Cos(40f * fLengthRatio);
                return fDepth;
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

            public static Vector3 vecGetTransformation(Vector3 vecPt)
            {
                float fNewX = vecPt.Y + 0.2f * vecPt.Z - 50f;
                float fNewY = 0.5f * vecPt.Z + 50f;
                float fNewZ = 0.5f * vecPt.X;
                Vector3 vecNewPt = new Vector3(fNewX, fNewY, fNewZ);
                return vecNewPt;
            }
        }
    }
}