// Spinning Wheel Canvas Module

export function drawWheel(canvas, segments) {
    const ctx = canvas.getContext('2d');
    const centerX = canvas.width / 2;
    const centerY = canvas.height / 2;
    const radius = Math.min(centerX, centerY) - 20;

    // Clear canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (!segments || segments.length === 0) {
        return;
    }

    // Draw segments
    segments.forEach((segment, index) => {
        drawSegment(ctx, centerX, centerY, radius, segment, index);
    });

    // Draw outer border
    ctx.beginPath();
    ctx.arc(centerX, centerY, radius, 0, 2 * Math.PI);
    ctx.strokeStyle = '#1e293b';
    ctx.lineWidth = 4;
    ctx.stroke();

    // Draw center circle
    ctx.beginPath();
    ctx.arc(centerX, centerY, 15, 0, 2 * Math.PI);
    ctx.fillStyle = '#1e293b';
    ctx.fill();

    // Draw center dot
    ctx.beginPath();
    ctx.arc(centerX, centerY, 5, 0, 2 * Math.PI);
    ctx.fillStyle = 'white';
    ctx.fill();
}

function drawSegment(ctx, centerX, centerY, radius, segment, index) {
    const startAngleRad = (segment.startAngle - 90) * Math.PI / 180; // -90 to start from top
    const endAngleRad = (segment.endAngle - 90) * Math.PI / 180;

    // Draw segment
    ctx.beginPath();
    ctx.moveTo(centerX, centerY);
    ctx.arc(centerX, centerY, radius, startAngleRad, endAngleRad);
    ctx.closePath();
    ctx.fillStyle = segment.color;
    ctx.fill();

    // Draw segment border
    ctx.strokeStyle = 'white';
    ctx.lineWidth = 2;
    ctx.stroke();

    // Draw text
    const textAngle = (segment.startAngle + segment.endAngle) / 2;
    const textAngleRad = (textAngle - 90) * Math.PI / 180;
    const textRadius = radius * 0.75;
    const textX = centerX + Math.cos(textAngleRad) * textRadius;
    const textY = centerY + Math.sin(textAngleRad) * textRadius;

    ctx.save();
    ctx.translate(textX, textY);
    ctx.rotate(textAngleRad + Math.PI / 2);

    // Calculate appropriate font size based on segment size
    const segmentAngle = segment.endAngle - segment.startAngle;
    const fontSize = Math.max(10, Math.min(18, segmentAngle / 360 * 100));

    ctx.fillStyle = 'white';
    ctx.font = `bold ${fontSize}px -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif`;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';

    // Add text shadow for better visibility
    ctx.shadowColor = 'rgba(0, 0, 0, 0.7)';
    ctx.shadowOffsetX = 1;
    ctx.shadowOffsetY = 1;
    ctx.shadowBlur = 2;

    // Truncate text if too long
    let text = segment.name;
    const maxWidth = segmentAngle / 360 * radius * 1.5;
    if (ctx.measureText(text).width > maxWidth) {
        while (ctx.measureText(text + '...').width > maxWidth && text.length > 1) {
            text = text.substring(0, text.length - 1);
        }
        text += '...';
    }

    ctx.fillText(text, 0, 0);
    ctx.restore();
}

export function spinWheel(canvas, targetAngle) {
    // Calculate final rotation - add multiple full rotations for effect
    const baseRotations = 3; // Minimum 3 full spins
    const extraRotations = Math.random() * 2; // Up to 2 additional rotations
    const totalRotations = baseRotations + extraRotations;
    const finalAngle = totalRotations * 360 + (360 - targetAngle); // 360 - targetAngle to align with pointer

    // Apply CSS transform for smooth animation
    canvas.style.transition = 'transform 3s cubic-bezier(0.23, 1, 0.32, 1)';
    canvas.style.transform = `rotate(${finalAngle}deg)`;

    // Reset after animation
    setTimeout(() => {
        canvas.style.transition = 'none';
        const normalizedAngle = finalAngle % 360;
        canvas.style.transform = `rotate(${normalizedAngle}deg)`;
    }, 3000);
}

export function clearWheel(canvas) {
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Reset any transforms
    canvas.style.transition = 'none';
    canvas.style.transform = 'rotate(0deg)';
}