/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["../../**/*.{cshtml,razor}"],
    theme: {
        extend: {
            colors: {
                "ogame-blue": "#87CEEB",
                "ogame-red": "#F08080",
                "ogame-amber": "#EEE8AA",
                "ogame-green": "#7FFFD4",
                "ogame-primary": "#E68318",
                "ogame-primary-darker": "#DE7E17",
                "ogame-secondary": "#7B1E00",
                "ogame-secondary-darker": "#661A00"
            }
        },
    },
    plugins: [],
};
