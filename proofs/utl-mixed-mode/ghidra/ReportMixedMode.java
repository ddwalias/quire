// Export a compact native-code inventory for the generated mixed-mode proof DLL.
import ghidra.app.script.GhidraScript;
import ghidra.program.model.listing.Function;
import ghidra.program.model.mem.MemoryBlock;

public class ReportMixedMode extends GhidraScript {
    @Override
    public void run() throws Exception {
        println("PROGRAM " + currentProgram.getName());
        println("IMAGE_BASE " + currentProgram.getImageBase());
        for (MemoryBlock block : currentProgram.getMemory().getBlocks()) {
            if (block.isExecute()) {
                println("EXECUTABLE_RANGE " + block.getName() + " " + block.getStart() + "-" + block.getEnd());
            }
        }
        for (Function function : currentProgram.getFunctionManager().getFunctions(true)) {
            if (function.getName().contains("NativeAdd") || function.getName().contains("NativeBridge")) {
                println("FUNCTION " + function.getEntryPoint() + " " + function.getName() + " " + function.getBody());
                println("SIGNATURE " + function.getSignature());
                var instruction = currentProgram.getListing().getInstructionAt(function.getEntryPoint());
                if (instruction != null) {
                    for (int count = 0; instruction != null && count < 12; count++) {
                        println("INSTRUCTION " + instruction.getAddress() + " " + instruction);
                        instruction = instruction.getNext();
                    }
                }
            }
        }
    }
}
