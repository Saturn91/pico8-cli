const test_output = window.test_output.split(window.lua_output_new_line);
const testOutputElement = document.getElementById("test-output-element");

const showPassed = document.getElementById("show-passed");

function updateLog(show_passed) {
    testOutputElement.innerHTML="";
    test_output.forEach(log_line => {
		const isStartLine = log_line.includes("[RUN ]: ")
        const isError = log_line.includes("error")
        const isPassed = log_line.includes("[PASS] -> ")
		const isEndLine = log_line.includes("[ OK ]: ")
        if ((isPassed && show_passed==true) || isError || isStartLine || isEndLine) {
            const logLineElement = document.createElement("p")
        
            isError && logLineElement.classList.add("error-line");
            (isPassed || isStartLine || isEndLine) && logLineElement.classList.add("info-line"); 
        
            logLineElement.innerText = log_line;
            testOutputElement.appendChild(logLineElement)
        }
        
    });
}

showPassed.addEventListener('change', (event) => {
    updateLog(event.target.checked);
})

updateLog(true);

