{
    "Schema": {
        "Type": "Behaviour",
        "Parents": [],
        "Datas": [],
        "Events": {
            "BeginExecution": {
                "1": {
                    "Type": "ExecutionEnter",
                    "ExecEnterNodes": [],
                    "ExecOutNodes": [
                        2
                    ],
                    "Args": [],
                    "Returns": [
                        2
                    ]
                },
                "2": {
                    "Type": "Call",
                    "ExecEnterNodes": [],
                    "ExecOutNodes": [],
                    "Args": [
                        3
                    ],
                    "Returns": []
                },
                "3": {
                    "Type": "Value",
                    "ExecEnterNodes": [],
                    "ExecOutNodes": [],
                    "Returns": [ "Hello World from GryphonSharp!" ]
                }
            }
        }
    }
}