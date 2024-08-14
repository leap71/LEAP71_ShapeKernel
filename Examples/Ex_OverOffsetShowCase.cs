//
// SPDX-License-Identifier: CC0-1.0
//
// This example code file is released to the public under Creative Commons CC0.
// See https://creativecommons.org/publicdomain/zero/1.0/legalcode
//
// To the extent possible under law, LEAP 71 has waived all copyright and
// related or neighboring rights to this PicoGK Example Code.
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
    using ShapeKernel;

    namespace ShapeKernelExamples
    {
        class OverOffsetShowCase
        {
            public static void Task()
            {
                //Step 1: Generate a simple shape with convex and concave corners
                BaseBox oBox_01             = new BaseBox(new LocalFrame(), 10, 40, 30);
                BaseBox oBox_02             = new BaseBox(new LocalFrame(), 40, 40, 10);
                Voxels voxBox               = Sh.voxUnion(oBox_01.voxConstruct(), oBox_02.voxConstruct());
                

                //Step 2: Apply one of the below offset operations
                //Voxels voxNewBox            = Sh.voxOffset(voxBox, 3);
                Voxels voxNewBox          = Sh.voxOverOffset(voxBox, 3, 0);
                //Voxels voxNewBox          = Sh.voxOverOffset(voxBox, -3, 0);
                //Voxels voxNewBox            = Sh.voxSmoothen(voxBox, 3);


                //Step 3: Visualize
                Sh.PreviewBoxWireframe(oBox_01, Cp.clrBlack);
                Sh.PreviewBoxWireframe(oBox_02, Cp.clrBlack);
                Sh.PreviewVoxels(voxNewBox, Cp.clrBubblegum, 1.0f);
            }
        }
    }
}