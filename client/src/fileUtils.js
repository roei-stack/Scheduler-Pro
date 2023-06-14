/*
CourseID, CourseName, LecturePoints, TAPoints, LectureOccurrences, TAOccurrences, LectureParts, TAAfterLecture

- `CourseID`: A unique identifier for the course.
- `CourseName`: The name of the course.
- `LecturePoints`: The number of points/credits assigned to the lecture.
- `TAPoints`: The number of points/credits assigned to the teaching assistant (TA).
- `LectureOccurrences`: A dictionary or integer specifying the lecture occurrences for different semesters. It can be provided as a dictionary in the format `{A: value, B: value, Summer: value}` or as an integer that will be applied to all semesters.
- `TAOccurrences`: A dictionary or integer specifying the TA occurrences for different semesters. It follows the same format as `LectureOccurrences`.
- `LectureParts`: The number of parts or sessions the lecture is divided into during the week.
- `TAAfterLecture` (optional): The number of times the TA comes after the lecture during the week. If not provided, it defaults to 0.

Here's an example of how a `course.txt` file can be prepared:

MATH202,Calculus II,4,3,{A:3;B:3;Summer:3},{A:3;B:3;Summer:3},3,2
CHEM101,General Chemistry,4,2,3,3,2
PHYS201,Physics I,4,3,{A:3;B:3;Summer:2},3,3,1
COMP301,Introduction to Computer Science,3,2,{A:3;B:3;Summer:2},1, 1

Ensure that each line follows the specified format, with the values properly separated by commas.
*/
export const parseCoursesFileContent = fileContent => {
    const lines = fileContent.split('\n');
    const courses = [];
    const parsedCourseIds = new Set();

    for (let i = 0; i < lines.length; i++) {
        const line = lines[i].trim();
        // ignore empty lines
        if (line === '') continue;

        const parts = line.split(',');

        if (parts.length < 7 || parts.length > 8) {
            throw new Error(`Invalid number of fields on line ${i + 1}`);
        }

        if (parsedCourseIds.has(parts[0].trim())) {
            throw new Error(`Duplicate course ID '${parts[0].trim()}' at line ${i + 1}`);
        }
        try {
            const course = {
                id: parseString(parts[0], 'id'),
                name: parseString(parts[1], 'name'),
                lecture_points: parseInteger(parts[2], 'lecture_points'),
                TA_points: parseInteger(parts[3], 'TA_points'),
                lecture_occurrences: parseDictionary(parts[4], 'lecture_occurrences'),
                TA_occurrences: parseDictionary(parts[5], 'TA_occurrences'),
                lecture_parts: parseInteger(parts[6], 'lecture_parts'),
                TA_after_lecture: parts[7] ? parseInteger(parts[7], 'TA_after_lecture') : 0,
            };
            parsedCourseIds.add(course.id);
            courses.push(course);
        } catch (error) {
            throw new Error(`Failed to parse course at line ${i + 1}: ${error.message}`);
        }
    }
    return courses;
};

const parseString = (value, fieldName) => {
    const parsedValue = value.trim();
    if (parsedValue === '') {
        throw new Error(`Empty value found for field '${fieldName}'`);
    }
    return parsedValue;
};

const parseInteger = (value, fieldName) => {
    const parsedValue = parseInt(value.trim(), 10);

    if (isNaN(parsedValue)) {
        throw new Error(`Invalid value '${value}' found for field '${fieldName}', expected an integer`);
    } else if (parsedValue < 0) {
        throw new Error(`Negative value found for field '${fieldName}`);
    }
    return parsedValue;
};

const parseDictionary = (dictionaryString, fieldName) => {
    const trimmed = dictionaryString.trim();
    if (!trimmed.startsWith('{') && !trimmed.endsWith('}')) {
        const occurrences = parseInt(trimmed.trim(), 10);
        if (isNaN(occurrences)) {
            throw new Error(`Invalid value '${dictionaryString}' found for feild ${fieldName}`);
        } else if (occurrences < 0) {
            throw new Error(`Negative value '${dictionaryString}' found for feild ${fieldName}`);
        }
        return {
            A: occurrences,
            B: occurrences,
            Summer: occurrences,
        };
    }
    try {
        const dictionaryObj = JSON.parse(trimmed.replace(/;/g, ',').replace(/([{,])(\w+):/g, '$1"$2":'));
        if (
            typeof dictionaryObj === 'object' &&
            Object.keys(dictionaryObj).length === 3 &&
            'A' in dictionaryObj &&
            'B' in dictionaryObj &&
            'Summer' in dictionaryObj
        ) {
            const parsedOccurrences = {};
            for (const key in dictionaryObj) {
                const semester = key.trim().toLowerCase();
                const value = dictionaryObj[key];
                if (typeof value !== 'number' || value < 0) {
                    throw new Error(`Invalid value '${value}' for semester '${semester}' in occurrences field '${fieldName}'`);
                }
                parsedOccurrences[semester] = value;
            }
            return parsedOccurrences;
        } else {
            throw new Error(`Invalid occurrences format for feild ${fieldName}`);
        }
    } catch (error) {
        throw new Error(`Invalid occurrences format for feild ${fieldName}`);
    }
};

export const parseStaffFileContent = fileContent => {
    
}