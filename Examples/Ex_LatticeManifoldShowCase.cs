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
        class LatticeManifoldShowCase
        {
            public static void Task()
            {
                try
                {
                    {
                        float fOverhangAngleInDeg   = 45;
                        bool bExtendBothSides       = false;
                        float fLength               = 50;
                        float fRadius               = 5;
                        LocalFrame oLocalFrame      = new LocalFrame(new Vector3(-50, 0, 0), Vector3.UnitY);
                        LatticeManifold oShape      = new LatticeManifold(oLocalFrame, fLength, fRadius, fOverhangAngleInDeg, bExtendBothSides);
                        Voxels oVoxels              = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrYellow);
                    }

                    {
                        float fOverhangAngleInDeg   = 30;
                        bool bExtendBothSides       = true;
                        float fLength               = 50;
                        float fRadius               = 10;
                        LocalFrame oLocalFrame      = new LocalFrame(new Vector3(0, 0, 0), Vector3.UnitY);
                        LatticeManifold oShape      = new LatticeManifold(oLocalFrame, fLength, fRadius, fOverhangAngleInDeg, bExtendBothSides);
                        Voxels oVoxels              = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrCrystal);
                    }

                    {
                        float fOverhangAngleInDeg   = 60;
                        bool bExtendBothSides       = true;
                        float fLength               = 50;
                        float fRadius               = 5;
                        LocalFrame oLocalFrame      = new LocalFrame(new Vector3(50, 0, 0), Vector3.UnitY);
                        LatticeManifold oShape      = new LatticeManifold(oLocalFrame, fLength, fRadius, fOverhangAngleInDeg, bExtendBothSides);
                        Voxels oVoxels              = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrGreen);
                    }

                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }
        }
    }
}