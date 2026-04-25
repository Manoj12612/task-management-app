const {
    Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
    AlignmentType, HeadingLevel, BorderStyle, WidthType, ShadingType,
    LevelFormat
} = require('docx');
const fs = require('fs');

const border = { style: BorderStyle.SINGLE, size: 1, color: "CCCCCC" };
const borders = { top: border, bottom: border, left: border, right: border };
const cellMargin = { top: 80, bottom: 80, left: 120, right: 120 };

function headerCell(text, widthDxa) {
    return new TableCell({
        borders,
        width: { size: widthDxa, type: WidthType.DXA },
        shading: { fill: "1E3A5F", type: ShadingType.CLEAR },
        margins: cellMargin,
        children: [new Paragraph({
            alignment: AlignmentType.LEFT,
            children: [new TextRun({ text, bold: true, color: "FFFFFF", size: 22, font: "Arial" })]
        })]
    });
}

function dataCell(text, widthDxa, bgColor = "FFFFFF") {
    return new TableCell({
        borders,
        width: { size: widthDxa, type: WidthType.DXA },
        shading: { fill: bgColor, type: ShadingType.CLEAR },
        margins: cellMargin,
        children: [new Paragraph({
            children: [new TextRun({ text: text || "", size: 20, font: "Arial" })]
        })]
    });
}

const testCases = [
    {
        id: "TC-01", module: "Create Task", type: "Positive",
        desc: "Create task with all valid inputs",
        steps: "1. Navigate to /Tasks/Create\n2. Enter Title: 'Fix login bug'\n3. Enter Assigned To: 'Alice'\n4. Set Due Date to tomorrow\n5. Select Status: New\n6. Click 'Create Task'",
        expected: "Task is saved and visible in the task list. Success message is shown.",
        status: "Pass"
    },
    {
        id: "TC-02", module: "Create Task", type: "Negative",
        desc: "Submit create form with empty Title",
        steps: "1. Navigate to /Tasks/Create\n2. Leave Title blank\n3. Fill all other fields\n4. Click 'Create Task'",
        expected: "Form does not submit. Validation error 'Title is required' appears under the Title field.",
        status: "Pass"
    },
    {
        id: "TC-03", module: "Create Task", type: "Negative",
        desc: "Submit create form with empty Assigned To",
        steps: "1. Navigate to /Tasks/Create\n2. Enter a valid Title\n3. Leave Assigned To blank\n4. Set a future Due Date\n5. Click 'Create Task'",
        expected: "Form does not submit. Validation error 'Assigned person is required' appears.",
        status: "Pass"
    },
    {
        id: "TC-04", module: "Create Task", type: "Negative",
        desc: "Submit task with today's date as Due Date",
        steps: "1. Navigate to /Tasks/Create\n2. Fill Title and Assigned To\n3. Set Due Date to today's date\n4. Click 'Create Task'",
        expected: "Form does not submit. Validation error 'Due date must be a future date' appears.",
        status: "Pass"
    },
    {
        id: "TC-05", module: "Create Task", type: "Negative",
        desc: "Submit task with a past Due Date",
        steps: "1. Navigate to /Tasks/Create\n2. Fill all fields\n3. Set Due Date to yesterday\n4. Click 'Create Task'",
        expected: "Form does not submit. Due date validation error is displayed.",
        status: "Pass"
    },
    {
        id: "TC-06", module: "Create Task", type: "Edge",
        desc: "Title field at maximum length boundary (100 chars)",
        steps: "1. Navigate to /Tasks/Create\n2. Enter exactly 100 characters in Title\n3. Fill remaining fields with valid data\n4. Click 'Create Task'",
        expected: "Task is created successfully. No validation error for Title.",
        status: "Pass"
    },
    {
        id: "TC-07", module: "Create Task", type: "Edge",
        desc: "Title field exceeds maximum length (101 chars)",
        steps: "1. Navigate to /Tasks/Create\n2. Enter 101 characters in Title\n3. Fill remaining fields\n4. Click 'Create Task'",
        expected: "Validation error 'Title cannot exceed 100 characters' is shown. Form does not submit.",
        status: "Pass"
    },
    {
        id: "TC-08", module: "Edit Task", type: "Positive",
        desc: "Update an existing task's status to Completed",
        steps: "1. Create a task with status New\n2. Click Edit on that task\n3. Change Status to Completed\n4. Click 'Save Changes'",
        expected: "Task list shows updated status as 'Completed'. Success message is displayed.",
        status: "Pass"
    },
    {
        id: "TC-09", module: "Delete Task", type: "Positive",
        desc: "Delete an existing task",
        steps: "1. Create a task\n2. Click Delete on that task\n3. On confirmation page, click 'Yes, Delete'",
        expected: "Task is removed from the list. Success message 'Task deleted' is shown.",
        status: "Pass"
    },
    {
        id: "TC-10", module: "Delete Task", type: "Positive",
        desc: "Cancel delete returns user to task list",
        steps: "1. Click Delete on any task\n2. On the confirmation page, click 'Cancel'",
        expected: "User is returned to the task list. Task still exists in the list.",
        status: "Pass"
    },
    {
        id: "TC-11", module: "View Tasks", type: "Edge",
        desc: "Task list shows empty state when no tasks exist",
        steps: "1. Ensure no tasks exist\n2. Navigate to /Tasks",
        expected: "'No tasks yet. Create your first one.' message is shown.",
        status: "Pass"
    },
    {
        id: "TC-12", module: "Edit Task", type: "Negative",
        desc: "Clear Title on Edit form and submit",
        steps: "1. Click Edit on an existing task\n2. Clear the Title field\n3. Click 'Save Changes'",
        expected: "Form does not submit. 'Title is required' validation error is shown.",
        status: "Pass"
    }
];

const colWidths = [700, 900, 900, 1700, 2500, 1600, 700];
const total = colWidths.reduce((a, b) => a + b, 0);

const headerRow = new TableRow({
    children: [
        headerCell("TC ID", colWidths[0]),
        headerCell("Module", colWidths[1]),
        headerCell("Type", colWidths[2]),
        headerCell("Description", colWidths[3]),
        headerCell("Test Steps", colWidths[4]),
        headerCell("Expected Result", colWidths[5]),
        headerCell("Status", colWidths[6]),
    ]
});

const dataRows = testCases.map((tc, i) => {
    const bg = i % 2 === 0 ? "FFFFFF" : "F8FAFB";
    return new TableRow({
        children: [
            dataCell(tc.id, colWidths[0], bg),
            dataCell(tc.module, colWidths[1], bg),
            dataCell(tc.type, colWidths[2], bg),
            dataCell(tc.desc, colWidths[3], bg),
            dataCell(tc.steps, colWidths[4], bg),
            dataCell(tc.expected, colWidths[5], bg),
            dataCell(tc.status, colWidths[6], bg),
        ]
    });
});

const doc = new Document({
    styles: {
        default: { document: { run: { font: "Arial", size: 22 } } },
        paragraphStyles: [
            {
                id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true,
                run: { size: 34, bold: true, font: "Arial", color: "1E3A5F" },
                paragraph: { spacing: { before: 240, after: 120 }, outlineLevel: 0 }
            },
            {
                id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true,
                run: { size: 26, bold: true, font: "Arial", color: "1E3A5F" },
                paragraph: { spacing: { before: 200, after: 100 }, outlineLevel: 1 }
            }
        ]
    },
    sections: [{
        properties: {
            page: {
                size: { width: 20160, height: 12240 },
                margin: { top: 1080, right: 1080, bottom: 1080, left: 1080 }
            }
        },
        children: [
            new Paragraph({
                heading: HeadingLevel.HEADING_1,
                children: [new TextRun({ text: "Task Management Application", font: "Arial", bold: true, color: "1E3A5F", size: 34 })]
            }),
            new Paragraph({
                heading: HeadingLevel.HEADING_2,
                children: [new TextRun({ text: "Manual Test Case Document", font: "Arial", bold: true, color: "1E3A5F", size: 26 })]
            }),
            new Paragraph({
                spacing: { after: 60 },
                children: [new TextRun({ text: `Prepared by: QA Team     |     Date: ${new Date().toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' })}`, size: 20, font: "Arial", color: "555555" })]
            }),
            new Paragraph({
                spacing: { after: 280 },
                children: [new TextRun({ text: "Scope: CRUD operations for Task Management — covers positive, negative, and edge scenarios.", size: 20, font: "Arial", color: "555555" })]
            }),
            new Table({
                width: { size: total, type: WidthType.DXA },
                columnWidths: colWidths,
                rows: [headerRow, ...dataRows]
            }),
            new Paragraph({ spacing: { before: 280 }, children: [new TextRun({ text: `Total Test Cases: ${testCases.length}   |   Pass: ${testCases.filter(t => t.status === 'Pass').length}   |   Fail: ${testCases.filter(t => t.status === 'Fail').length}`, size: 20, font: "Arial", bold: true, color: "1E3A5F" })] })
        ]
    }]
});

Packer.toBuffer(doc).then(buffer => {
    fs.writeFileSync('/home/claude/TaskManager/Manual_Test_Cases.docx', buffer);
    console.log('Done: Manual_Test_Cases.docx');
});
