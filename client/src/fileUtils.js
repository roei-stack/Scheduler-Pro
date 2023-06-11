export const parseCoursesFileContent = fileContent => {
    const lines = fileContent.split('\n');
    const courses = [];

    for (let i = 0; i < lines.length; i++) {
        const line = lines[i].trim();

        // Skip empty lines
        if (line === '') continue;

        const parts = line.split(',');
        if (parts.length !== 4) {
            throw new Error(`Invalid file format. Expected four values per line, seperated by a comma. Line ${i + 1}: ${line}`);
        }

        const courseId = parts[0].trim().toLowerCase();
        const courseName = parts[1].trim().toLowerCase();
        const lectureTimes = parseTimes(parts[2].trim().replace(/^"(.*)"$/, '$1').toLowerCase(), i + 1);
        const exerciseTimes = parseTimes(parts[3].trim().replace(/^"(.*)"$/, '$1').toLowerCase(), i + 1);

        const course = {
            courseId,
            courseName,
            lectureTimes,
            exerciseTimes,
        };

        courses.push(course);
    }
    return courses;
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
