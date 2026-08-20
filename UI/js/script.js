// =====================================================
// API CONFIGURATION
// =====================================================

const API_BASE_URL = "https://devportfolio-api-manish2026-hhbvdna5b5cpgne6.centralindia-01.azurewebsites.net/api";

const CONTACT_API = `${API_BASE_URL}/Contact`;
const PROJECT_API = `${API_BASE_URL}/Projects`;


// =====================================================
// DOCUMENT READY
// =====================================================

$(document).ready(function () {

    console.log("Portfolio UI loaded.");

    // Current year
    $("#year").text(new Date().getFullYear());

    // Load projects
    loadProjects();

    // Contact form
    $("#contactForm").on(
        "submit",
        submitContactForm
    );

    // Smooth scrolling
    $('a[href^="#"]').on(
        "click",
        function (event) {

            const target =
                $(this.hash);

            if (target.length) {

                event.preventDefault();

                $("html, body").animate(
                    {
                        scrollTop:
                            target.offset().top - 70
                    },
                    700
                );
            }
        }
    );


    // Navbar background
    $(window).on(
        "scroll",
        function () {

            if ($(window).scrollTop() > 50) {

                $(".navbar").css({
                    "background": "#050a14",
                    "box-shadow":
                        "0 5px 20px rgba(0,0,0,0.3)"
                });

            }
            else {

                $(".navbar").css({
                    "background":
                        "rgba(11, 17, 32, 0.95)",
                    "box-shadow": "none"
                });
            }
        }
    );


    // Mobile navbar
    $(".navbar-nav .nav-link").on(
        "click",
        function () {

            $(".navbar-collapse").collapse("hide");

        }
    );

});


// =====================================================
// CONTACT FORM
// =====================================================

async function submitContactForm(event) {

    event.preventDefault();

    console.log("Contact form submitted.");


    // =================================================
    // GET VALUES
    // =================================================

    const name =
        $("#name").val().trim();

    const email =
        $("#email").val().trim();

    const subject =
        $("#subject").val().trim();

    const message =
        $("#message").val().trim();


    // =================================================
    // VALIDATION
    // =================================================

    if (name === "") {

        showFormMessage(
            "Please enter your name.",
            "danger"
        );

        return;
    }


    if (email === "") {

        showFormMessage(
            "Please enter your email.",
            "danger"
        );

        return;
    }


    if (subject === "") {

        showFormMessage(
            "Please enter subject.",
            "danger"
        );

        return;
    }


    if (message === "") {

        showFormMessage(
            "Please enter your message.",
            "danger"
        );

        return;
    }


    // =================================================
    // SUBMIT BUTTON
    // =================================================

    const submitButton =
        $("#contactForm button[type='submit']");


    submitButton.prop(
        "disabled",
        true
    );

    submitButton.text(
        "Sending..."
    );


    $("#formMessage").html("");


    // =================================================
    // API REQUEST
    // =================================================

    try {

        console.log(
            "Sending contact request to:",
            CONTACT_API
        );


        const response =
            await fetch(
                CONTACT_API,
                {
                    method: "POST",

                    headers: {
                        "Content-Type":
                            "application/json"
                    },

                    body: JSON.stringify({

                        name: name,

                        email: email,

                        subject: subject,

                        message: message

                    })
                }
            );


        console.log(
            "Contact API status:",
            response.status
        );


        // =================================================
        // SUCCESS
        // =================================================

        if (response.ok) {

            let result = null;

            try {

                result =
                    await response.json();

            }
            catch (e) {

                console.log(
                    "Response is not JSON."
                );
            }


            showFormMessage(
                result?.message ||
                "Message sent successfully!",
                "success"
            );


            // Clear form
            $("#contactForm")[0].reset();

            return;
        }


        // =================================================
        // API ERROR
        // =================================================

        let errorMessage =
            "Unable to send message. Please try again.";


        try {

            const errorData =
                await response.json();


            if (errorData.message) {

                errorMessage =
                    errorData.message;
            }

            if (errorData.error) {

                console.error(
                    "API Error:",
                    errorData.error
                );
            }

        }
        catch (error) {

            console.error(
                "Unable to read API error:",
                error
            );
        }


        showFormMessage(
            errorMessage,
            "danger"
        );

    }


    // =================================================
    // NETWORK ERROR / CORS
    // =================================================

    catch (error) {

        console.error(
            "Contact form error:",
            error
        );


        showFormMessage(
            "Unable to connect to server. Please make sure ASP.NET Core API is running.",
            "danger"
        );
    }


    // =================================================
    // ENABLE BUTTON
    // =================================================

    finally {

        submitButton.prop(
            "disabled",
            false
        );

        submitButton.text(
            "Send Message"
        );
    }
}


// =====================================================
// FORM MESSAGE
// =====================================================

function showFormMessage(
    message,
    type
) {

    $("#formMessage").html(`

        <div class="alert alert-${type} mt-3">

            ${escapeHtml(message)}

        </div>

    `);

}


// =====================================================
// LOAD PROJECTS
// =====================================================

async function loadProjects() {

    const container =
        $("#projectsContainer");


    if (!container.length) {

        return;
    }


    try {

        const response =
            await fetch(
                PROJECT_API
            );


        if (!response.ok) {

            throw new Error(
                "Projects API failed. Status: " +
                response.status
            );
        }


        const projects =
            await response.json();


        container.empty();


        if (
            !projects ||
            projects.length === 0
        ) {

            container.html(`

                <div class="col-12">

                    <div class="projects-empty">

                        <h4>
                            No Projects Available
                        </h4>

                        <p>
                            Projects will appear here once
                            they are added from the admin dashboard.
                        </p>

                    </div>

                </div>

            `);

            return;
        }


        // =================================================
        // PROJECT CARDS
        // =================================================

        projects.forEach(
            function (project) {

                const title =
                    escapeHtml(
                        project.title ||
                        "Untitled Project"
                    );


                const description =
                    escapeHtml(
                        project.description ||
                        "Project description is not available."
                    );


                const technologies =
                    project.technologies ||
                    "";


                // =================================================
                // TECHNOLOGY BADGES
                // =================================================

                const technologyBadges =
                    technologies
                        .split(",")
                        .map(
                            function (tech) {

                                tech =
                                    tech.trim();


                                if (!tech) {

                                    return "";
                                }


                                return `

                                    <span class="technology-badge">

                                        ${escapeHtml(tech)}

                                    </span>

                                `;
                            }
                        )
                        .join("");


                // =================================================
                // PROJECT HTML
                // =================================================

                const projectHtml = `

                    <div class="col-lg-6 col-md-6 col-sm-12">

                        <article class="project-card">

                            <div class="project-content">

                                <div class="project-number">
                                    PROJECT
                                </div>

                                <h3 class="project-title">

                                    ${title}

                                </h3>

                                <p class="project-description">

                                    ${description}

                                </p>

                                ${
                                    technologyBadges
                                        ? `

                                            <div class="technology-list">

                                                ${technologyBadges}

                                            </div>

                                          `
                                        : ""
                                }

                            </div>

                        </article>

                    </div>

                `;


                container.append(
                    projectHtml
                );

            }
        );

    }
    catch (error) {

        console.error(
            "Projects API Error:",
            error
        );


        container.html(`

            <div class="col-12">

                <div class="projects-error">

                    <h4>
                        Unable to Load Projects
                    </h4>

                    <p>
                        Please try again later.
                    </p>

                </div>

            </div>

        `);
    }
}


// =====================================================
// HTML SECURITY
// =====================================================

function escapeHtml(value) {

    return String(value)

        .replace(
            /&/g,
            "&amp;"
        )

        .replace(
            /</g,
            "&lt;"
        )

        .replace(
            />/g,
            "&gt;"
        )

        .replace(
            /"/g,
            "&quot;"
        )

        .replace(
            /'/g,
            "&#039;"
        );
}