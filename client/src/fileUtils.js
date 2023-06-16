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
                lecture_occurrences: parseDictionary(parts[4], 'lecture_occurrences', `${parts[0].trim()}_lecture`),
                TA_occurrences: parseDictionary(parts[5], 'TA_occurrences', `${parts[0].trim()}_TA`),
                lecture_parts: parseInteger(parts[6], 'lecture_parts'),
                TA_after_lecture: parts[7] ? parseInteger(parts[7], 'TA_after_lecture') : 0,
            };
            if (course.lecture_points % course.lecture_parts !== 0) {
                throw new Error(`lecture points (${course.lecture_points}) is not devisible by lecture parts (${course.lecture_parts})`);
            }
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

const parseDictionary = (dictionaryString, fieldName, id) => {
    const trimmed = dictionaryString.trim();
    if (!trimmed.startsWith('{') && !trimmed.endsWith('}')) {
        const occurrences = parseInt(trimmed.trim(), 10);
        if (isNaN(occurrences)) {
            throw new Error(`Invalid value '${dictionaryString}' found for feild ${fieldName}`);
        } else if (occurrences < 0) {
            throw new Error(`Negative value '${dictionaryString}' found for feild ${fieldName}`);
        }
        return {
            a: occurrences,
            b: occurrences,
            summer: occurrences,
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

/*
To prepare the `staff.txt` file for parsing, follow the given structure:

LoneUniStaff:
- Start this section by the line "LoneUniStaff".
- Each line represents a lone university staff member.
- The format for each line is as follows:
  - Staff ID (string)
  - Staff Name (string)
  - Courses Roles Occurrences (dictionary)

Example:

LoneUniStaff
1,John Doe,{CS101: {Lecturer: 2} ; MATH101: {Lecturer: 1; TA: 1}}
2,Jane Smith,{MATH202: {Lecturer: 3} ; PHYS101: {Lecturer: 2}}
3,David Johnson,{CHEM101: {Lecturer: 2} ; BIOLOGY201: {Lecturer: 1; TA: 1}}

Each staff has an ID, name, and the courses they are associated with along with their respective roles and occurrences.

MultipleUniStaff:
- Start this section by the line "MultipleUniStaff".
- This section represents a group of university staff members sharing the same lecture.
- each line contains semicolon-separated staff IDs, a comma, and semicolon-separated course ID's.

Example:

MultipleUniStaff
1;2,MATH202;CS101

In this example, there is one group of university staff members.
It consists of staff with IDs 1 and 2, who share the courses MATH202 and CS101.
 */
export const parseStaffFileContent = fileContent => {
    const lines = fileContent.split('\n');
    const staffData = { loneUniStaff: [], multipleUniStaff: [] };
    let currentSection = null;
    const loneUniStaffIds = new Set();

    for (let i = 0; i < lines.length; i++) {
        const trimmed = lines[i].trim();

        if (trimmed === 'LoneUniStaff') {
            currentSection = 'loneUniStaff';
        } else if (trimmed === 'MultipleUniStaff') {
            currentSection = 'multipleUniStaff';
        } else if (trimmed) {
            const parts = trimmed.split(',');
            try {
                if (currentSection === 'loneUniStaff') {
                    if (parts.length !== 3) {
                        throw new Error(`LoneUniStaff: Invalid number of fields, expected 3`);
                    }
                    // check for duplicate staff id's
                    const id = parseString(parts[0], 'id');
                    if (loneUniStaffIds.has(id)) {
                        throw new Error(`LoneUniStaff: Duplicate Uni staff id ${id}`);
                    }

                    staffData.loneUniStaff.push({
                        id: id,
                        name: parseString(parts[1], 'name'),
                        coursesRolesOccurrences: parseCoursesRolesOccurrences(parts[2])
                    });
                    loneUniStaffIds.add(id);
                } else if (currentSection === 'multipleUniStaff') {
                    if (parts.length !== 2) {
                        throw new Error(`MultipleUniStaff: Invalid number of fields, expected 2`);
                    }
                    // check for duplicate staff id's in multipleUniStaff
                    const staffList = parts[0].trim().split(';').map(id => parseString(id, "staffList id's"));
                    if (staffList.length < 2 || new Set(staffList).size !== staffList.length) {
                        throw new Error(`MultipleUniStaff: Too short or Duplicate staff IDs`);
                    }
                    // check for duplicated shared courses
                    const sharedCourses = parts[1].trim().split(';').map(course => parseString(course, "shared courses list"));
                    if (sharedCourses.length < 1 || new Set(sharedCourses).size !== sharedCourses.length) {
                        throw new Error(`MultipleUniStaff: Empty or Duplicate course IDs`);
                    }
                    staffData.multipleUniStaff.push({
                        staffList: staffList,
                        sharedCourses: sharedCourses
                    });
                } else {
                    throw new Error('no section found (MultipleUniStaff/LoneUniStaff)')
                }
            } catch (error) {
                throw new Error(`Failed to parse staff at line ${i + 1}: ${error.message}`);
            }
        }
    }
    staffData.multipleUniStaff.forEach(mulUniStaff => {
        mulUniStaff.staffList.forEach(loneUniStaffId => {
            if (!loneUniStaffIds.has(loneUniStaffId)) {
                throw new Error(`Uni staff ID '${loneUniStaffId}' from multipleUniStaff ${mulUniStaff} does not exist`);
            }
        })
    })
    return staffData;
    // todo check for courses id validity
};

const parseCoursesRolesOccurrences = coursesRolesOccurrences => {
    try {
        const preparedStr = coursesRolesOccurrences.trim().replace(/(\w+):/g, '"$1":').replace(/;/g, ',');
        const dictionaryObj = JSON.parse(preparedStr);

        if (typeof dictionaryObj !== 'object' || Array.isArray(dictionaryObj)) {
            throw new Error(`Invalid format for CoursesRolesOccurrences. Dictionary must be provided.`);
        }

        const parsedOccurrences = {};

        for (const key in dictionaryObj) {
            if (typeof key !== 'string') {
                throw new Error(`Invalid format for CoursesRolesOccurrences. Invalid key: ${key}`);
            }

            const value = dictionaryObj[key];

            if (typeof value !== 'object' || Array.isArray(value)) {
                throw new Error(`Invalid format for CoursesRolesOccurrences. Invalid value for key '${key}': ${value}`);
            }

            const parsedValue = {};

            for (const role in value) {
                if (role !== 'Lecturer' && role !== 'TA') {
                    throw new Error(`Invalid format for CoursesRolesOccurrences. Invalid role for key '${key}': ${role}`);
                }

                const occurrences = value[role];

                if (typeof occurrences !== 'number' || occurrences < 0) {
                    throw new Error(`Invalid format for CoursesRolesOccurrences. Invalid occurrences for key '${key}', role '${role}': ${occurrences}`);
                }

                parsedValue[role] = occurrences;
            }
            parsedOccurrences[key] = parsedValue;
        }
        return parsedOccurrences;
    } catch (error) {
        throw new Error(`Invalid format for CoursesRolesOccurrences. Failed to parse the dictionary: ${coursesRolesOccurrences}`);
    }
};

/*
To prepare the majors.txt file for parsing, follow the given structure:

CourseBundles:
- Start this section by the line "CourseBundles".
- Each line represents a course bundle.
- The format for each line is: `bundleId, minCreditPoints, maxCreditPoints, year, courseId1;courseId2;courseId3;..;courseId_n.`
- `bundleId` is a unique identifier for the bundle.
- `minCreditPoints` and `maxCreditPoints` represent the minimum and maximum credit points for the bundle.
- `year` represents the minimum year a student can take this bundle.
- `courseId1;courseId2;courseId3;...` is a semicolon-separated list of course IDs included in the bundle.

Majors:
- Each line represents a major.
- The format for each line is: `majorName, bundleId1;bundleId2;bundleId3;...;bundle_i`
- The `majorName` is a unique identifier for the major.
- `bundleId1;bundleId2;bundleId3;...` is a semicolon-separated list of bundle IDs associated with the major.

Example File:

CourseBundles
CS_Bundle1, 4, 8, 1, CS101;CS102;CS201
CS_Bundle2, 6, 12, 2, CS201;CS202;CS301;CS302
Math_Bundle1, 6, 10, 1, MATH101;MATH102
Math_Bundle2, 6, 12, 2, MATH201;MATH202;MATH301
Physics_Bundle1, 8, 14, 1, PHYS101;PHYS102;PHYS201;PHYS202

Majors
Computer Science, CS_Bundle1;CS_Bundle2;Math_Bundle1
Mathematics, Math_Bundle1;Math_Bundle2;CS_Bundle1
Physics, Physics_Bundle1;Math_Bundle1
*/
export const parseMajorsFileContent = fileContent => {
    const lines = fileContent.split('\n');
    const MajorsData = { courseBundles: [], majors: [] };
    let currentSection = null;
    const bundleIds = new Set();
    const majorNames = new Set();

    for (let i = 0; i < lines.length; i++) {
        const line = lines[i].trim();
        // ignore empty lines
        if (line === '') continue;
        const parts = line.split(',');
        try {
            if (line === 'CourseBundles' || line === 'Majors') {
                currentSection = line;

            } else if (currentSection === 'CourseBundles') {
                if (parts.length !== 5) {
                    throw new Error(`CourseBundles: Invalid number of fields, expected 5`);
                }
                // check duplicate bundle id's
                const bundleId = parseString(parts[0], 'id');
                if (bundleIds.has(bundleId)) {
                    throw new Error(`CourseBundles: Duplicate bundle ID '${bundleId}'`);
                }
                // check duplicate courses id's
                const courses = parts[4].trim().split(';').map(cid => parseString(cid, "course ID's"));
                if (courses.length === 0 || new Set(courses).size !== courses.length) {
                    throw new Error(`CourseBundles: Empty or Duplicate course ID/s in bundle '${bundleId}'`);
                }
                MajorsData.courseBundles.push({
                    id: bundleId,
                    minCreditPoints: parseInteger(parts[1], 'minCreditPoints'),
                    maxCreditPoints: parseInteger(parts[2], 'maxCreditPoints'),
                    year: parseInteger(parts[3], 'year'),
                    courses: courses
                });
                bundleIds.add(bundleId);

            } else if (currentSection === 'Majors') {
                if (parts.length !== 2) {
                    throw new Error(`Majors: Invalid number of fields, expected 2`);
                }
                // check duplicate major name
                const majorName = parseString(parts[0], 'majorName');
                if (majorNames.has(majorName)) {
                    throw new Error(`Majors: Duplicate major name '${majorName}'`);
                }
                // check for duplicate bundle id's
                const bundles = parts[1].trim().split(';').map(bid => parseString(bid, "bundle ID's"));
                if (bundles.length === 0 || new Set(bundles).size !== bundles.length) {
                    throw new Error(`Majors: Duplicate bundle ID's in major '${majorName}'`);
                }
                MajorsData.majors.push({
                    majorName: majorName,
                    bundles: bundles
                });
                majorNames.add(majorName);
            } else {
                throw new Error('no section found (CourseBundles/Majors)');
            }
        } catch (error) {
            throw new Error(`Failed to parse majors at line ${i + 1}: ${error.message}`);
        }
    }
    // make sure bundles in majors exist
    MajorsData.majors.forEach(major => {
        major.bundles.forEach(bundleId => {
            if (!bundleIds.has(bundleId)) {
                throw new Error(`Bundle ID '${bundleId}' from major ${major.majorName} does not exist in courseBundles`);
            }
        })
    })
    return MajorsData;
    // todo check courses id's valid
};