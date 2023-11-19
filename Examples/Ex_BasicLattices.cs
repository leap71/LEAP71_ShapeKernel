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


using System.Numerics;
using PicoGK;


namespace Leap71
{
    using ShapeKernel;

    namespace ShapeKernelExamples
    {
        class BasicLattices
        {
            public static void Task()
            {
                try
                {
                    Lattice oLattice = new Lattice();

                    //add node
                    Vector3 vecPt0 = new Vector3(1, 5, -10);
                    float fRadius0 = 5;
                    oLattice.AddSphere(vecPt0, fRadius0);

                    //add beams
                    Vector3 vecPt1 = new Vector3(5, 3, 0);
                    float fRadius1 = 1;
                    Vector3 vecPt2 = new Vector3(-3, 0, 7);
                    float fRadius2 = 3;
                    oLattice.AddBeam(vecPt1, fRadius1, vecPt2, fRadius2, true);
                    oLattice.AddBeam(vecPt1, fRadius1, vecPt2, fRadius2, false);

                    Sh.PreviewLattice(oLattice, Cp.clrBlueberry);
                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }
        }
    }
}