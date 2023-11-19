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
        class BaseRingShowCase
        {
            public static void Task()
            {
                try
                {
                    {
                        //basic
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, -50, 0));
                        BaseRing oShape         = new BaseRing(oLocalFrame, 30, 8);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrFrozen);
                    }

                    {
                        //modulated 0
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, 50, 0));
                        BaseRing oShape         = new BaseRing(oLocalFrame, 30);
                        oShape.SetRadius(new SurfaceModulation(fGetRingRadius0));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrPitaya);
                    }

                    {
                        //modulated 1
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(50, 50, 0));
                        BaseRing oShape         = new BaseRing(oLocalFrame, 30);
                        oShape.SetRadius(new SurfaceModulation(fGetRingRadius1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrWarning);
                    }

                    {
                        //modulated 2
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(50, -50, 0));
                        BaseRing oShape         = new BaseRing(oLocalFrame, 30);
                        oShape.SetRadius(new SurfaceModulation(fGetRingRadius2));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrBlueberry);
                    }
                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }

            //functions for ring surface modulations
            protected static float fGetRingRadius0(float fPhi, float fAlpha)
            {
                float fRadius = 10f - 2f * MathF.Cos(5f * fPhi);
                return fRadius;
            }

            protected static float fGetRingRadius1(float fPhi, float fAlpha)
            {
                float fRadius = 10f + 3f * MathF.Cos(5f * fAlpha);
                return fRadius;
            }

            protected static float fGetRingRadius2(float fPhi, float fAlpha)
            {
                fPhi += 1f * fAlpha;
                float fRadius = 10f - 2f * MathF.Cos(5f * fPhi) + 3f * MathF.Cos(5f * fAlpha);
                return fRadius;
            }
        }
    }
}