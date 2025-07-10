window.binaryDb = {
    openDb: function (dbName, storeName) {
        return new Promise((resolve, reject) => {
            const req = indexedDB.open(dbName, 1);
            req.onupgradeneeded = e => {
                e.target.result.createObjectStore(storeName);
            };
            req.onsuccess = e => resolve({ db: e.target.result, storeName });
            req.onerror = e => reject(e);
        });
    },

    putFile: async function (dbName, storeName, key, arrayBuffer, mime) {
        const { db } = await this.openDb(dbName, storeName);
        const tx = db.transaction(storeName, "readwrite");
        const store = tx.objectStore(storeName);
        const blob = new Blob([arrayBuffer], { type: mime });
        store.put(blob, key);
        return new Promise((res, rej) => {
            tx.oncomplete = () => res();
            tx.onerror = e => rej(e);
        });
    },

    getFile: async function (dbName, storeName, key) {
        const { db } = await this.openDb(dbName, storeName);
        const tx = db.transaction(storeName, "readonly");
        const store = tx.objectStore(storeName);
        const req = store.get(key);
        return new Promise((res, rej) => {
            req.onsuccess = e => res(e.target.result);
            req.onerror = e => rej(e);
        });
    },

    getFileAsBytes: async function (dbName, storeName, key) {
        const dbReq = indexedDB.open(dbName, 1);

        return new Promise((resolve, reject) => {
            dbReq.onerror = () => reject("Failed to open DB");

            dbReq.onsuccess = async function () {
                const db = dbReq.result;
                const tx = db.transaction(storeName, "readonly");
                const store = tx.objectStore(storeName);
                const req = store.get(key);

                req.onsuccess = async function () {
                    const blob = req.result;
                    if (!blob) {
                        reject("No file found");
                        return;
                    }

                    // Convert blob to ArrayBuffer
                    const arrayBuffer = await blob.arrayBuffer();
                    resolve(new Uint8Array(arrayBuffer)); // Send back to Blazor
                };

                req.onerror = () => reject("Failed to get file");
            };
        });
    },

    deleteFile: async function (dbName, storeName, key) {
        const dbReq = indexedDB.open(dbName, 1);

        return new Promise((resolve, reject) => {
            dbReq.onerror = () => reject("Failed to open DB");

            dbReq.onsuccess = function () {
                const db = dbReq.result;
                const tx = db.transaction(storeName, "readwrite");
                const store = tx.objectStore(storeName);

                const deleteRequest = store.delete(key);

                deleteRequest.onsuccess = () => resolve(true);
                deleteRequest.onerror = () => reject("Failed to delete entry");
            };
        });
    }
};