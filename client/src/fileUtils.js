// id,name,lecture_points,TA_points,lecture_occurrences,TA_occurrences,lecture_parts,TA_after_lecture
export const parseCoursesFileContent = fileContent => {
    const lines = fileContent.split('\n');
    const courses = [];

    for (let i = 0; i < lines.length; i++) {
        const line = lines[i].trim();
        // ignore empty lines
        if (line === '') continue;

        const parts = line.split(',');

        if (parts.length < 7 || parts.length > 8) {
            throw new Error(`Invalid number of fields on line ${i + 1}`);
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

const parseTimes = (timesString, line) => {
    const times = timesString.split(';').map(time => time.trim());
    const parsedTimes = [];

    for (const time of times) {
        // format: [start-hour]-[end-hour] on [day]
        // for example: 12-14 on sunday
        const match = time.match(/^(\d{1,2})-(\d{1,2}) on (\w+)$/);

        if (!match) {
            throw new Error(`Invalid time format on line ${line}. Expected 'start-end on day'. Time: ${time}`);
        }

        const startHour = parseInt(match[1], 10);
        const endHour = parseInt(match[2], 10);
        const day = match[3];

        if (startHour >= endHour) {
            throw new Error(`Invalid time range on line ${line}. Start hour must be before end hour. Time: ${time}`);
        }

        const parsedTime = {
            startHour,
            endHour,
            day,
        };

        parsedTimes.push(parsedTime);
    }
    return parsedTimes;
};
