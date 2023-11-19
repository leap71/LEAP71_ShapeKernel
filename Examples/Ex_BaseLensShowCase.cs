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
        class BaseLensShowCase
        {
            public static void Task()
            {
                try
                {
                    {
                        //basic
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, -50, 0));
                        BaseLens oShape         = new BaseLens(oLocalFrame, 10, 10, 40);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrFrozen);
                    }

                    {
                        //modulated 0
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(50, 50, 0));
                        BaseLens oShape         = new BaseLens(oLocalFrame, 10, 10, 40);
                        oShape.SetHeight(new SurfaceModulation(fGetLenseHeight1), new SurfaceModulation(fGetLenseHeight2));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrPitaya);
                    }

                    {
                        //modulated 1
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, 50, 0));
                        BaseLens oShape         = new BaseLens(oLocalFrame, 10, 10, 40);
                        oShape.SetHeight(new SurfaceModulation(fGetLenseHeight1), new SurfaceModulation(fGetLenseHeight3));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrWarning);
                    }
                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }

            //functions for lense surface modulations
            protected static float fGetLenseHeight1(float fPhi, float fRadiusRatio)
            {
                float fRadius = 5f - fGetSurfaceModulation(fPhi, fRadiusRatio);
                return fRadius;
            }

            protected static float fGetLenseHeight2(float fPhi, float fRadiusRatio)
            {
                float fRadius = 5f + fGetSurfaceModulation(fPhi, fRadiusRatio);
                return fRadius;
            }

            protected static float fGetLenseHeight3(float fPhi, float fRadiusRatio)
            {
                fPhi += 0.3f * MathF.PI * fRadiusRatio;
                float fRadius = 5f + 1f * MathF.Cos(6f * fPhi) + 3f * MathF.Cos(20f * fRadiusRatio);
                return fRadius;
            }

            //functions for generic surface modulations
            public static float fGetSurfaceModulation(float fPhi, float fLengthRatio)
            {
                float fRadius = 12f + 3f * MathF.Cos(5f * fPhi);
                return fRadius;
            }
        }
    }
}