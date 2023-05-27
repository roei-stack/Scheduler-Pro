import { useState } from 'react';
import './RatingStars.css'

const RatingStars = ({ totalStars = 5 }) => {
    const [selectedStars, setSelectedStars] = useState(0);
    const [hoveredStars, setHoveredStars] = useState(0);

    const handleStarClick = (index) => {
        setSelectedStars(index + 1);
    };

    const handleStarHover = (index) => {
        setHoveredStars(index + 1);
    }

    const handleStarStopHover = () => {
        setHoveredStars(0);
    }

    return (
        <>
            {[...Array(totalStars)].map((_, index) => (
                <i
                    key={index}
                    className={`fa-sharp fa-star fa-${(hoveredStars === 0 ? selectedStars > index : hoveredStars > index) ? 'solid' : 'regular'}`}
                    onClick={() => handleStarClick(index)}
                    onMouseEnter={() => { handleStarHover(index) }}
                    onMouseLeave={() => { handleStarStopHover() }}
                />
            ))}
        </>
    );
};

export default RatingStars;