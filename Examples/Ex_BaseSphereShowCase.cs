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
        class BaseSphereShowCase
        {
            public static void Task()
            {
                try
                {
                    {
                        //basic
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-100, 0, 0));
                        BaseSphere oShape       = new BaseSphere(oLocalFrame, 40);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrFrozen);
                    }

                    {
                        //modulated 0
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(0, 0, 0));
                        BaseSphere oShape       = new BaseSphere(oLocalFrame);
                        oShape.SetRadius(new SurfaceModulation(fGetSphereRadius0));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrPitaya);
                    }

                    {
                        //modulated 1
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(150, 0, 0));
                        BaseSphere oShape       = new BaseSphere(oLocalFrame);
                        oShape.SetRadius(new SurfaceModulation(fGetSphereRadius1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrWarning);
                    }
                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }

            //functions for sphere surface modulations
            protected static float fGetSphereRadius0(float fTheta, float fPhi)
            {
                float fRadius = 40f - 10f * MathF.Cos(6f * fPhi);
                return fRadius;
            }

            protected static float fGetSphereRadius1(float fTheta, float fPhi)
            {
                float fRadius = 40f - 10f * MathF.Cos(6f * fPhi) + 30f * MathF.Cos(2f * fTheta);
                return fRadius;
            }
        }
    }
}