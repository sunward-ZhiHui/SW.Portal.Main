window.fetchFile = async (url) => {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error("Failed to fetch file.");
        }
        const blob = await response.blob();
        return blob;
    } catch (error) {
        console.error("fetchFile error:", error);
        return null;
    }
};
